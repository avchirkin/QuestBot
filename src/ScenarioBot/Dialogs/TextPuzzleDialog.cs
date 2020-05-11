using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using ScenarioBot.Domain;

namespace ScenarioBot.Dialogs
{
    public class TextPuzzleDialog : CancelAndHelpDialog
    {
        public TextPuzzleDialog(IList<IBotCommand> botCommands,
            string id = "TextPuzzleDialog"
        )
            : base(id, botCommands)
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));

            var waterfallStep = new WaterfallStep[]
            {
                AskDialog,
                CheckDialog
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        protected virtual async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;

            var reply = stepContext.Context.Activity.CreateReply(puzzleDetails.Question);
            if (puzzleDetails.ShowPossibleBranches)
            {   
                reply.SuggestedActions = new SuggestedActions()
                {
                    Actions = puzzleDetails.PossibleAnswers
                        .Select(name => new CardAction() {Title = name, Type = ActionTypes.ImBack, Value = name})
                        .ToList()
                };
            }
            else
            {
                // хак скриыть клавиатуру в тг
                GenerateHideKeyboardMarkupForTelegram(reply);
            }

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);
            return new DialogTurnResult(DialogTurnStatus.Waiting);
            
            //return await stepContext.PromptAsync(nameof(TextPrompt),
            //    new PromptOptions {Prompt = MessageFactory.Text(puzzleDetails.Question)}, cancellationToken);

        }

        protected virtual async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;
            var answer = (string) stepContext.Result;
            puzzleDetails.SetAnswer(answer);
            
            if (puzzleDetails.IsRight) return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);

            if (puzzleDetails.NumberOfAttempts >= puzzleDetails.NumberOfAttemptsLimit)
            {
                var message = "К сожалению, количество попыток дать правильный ответ закончилось";
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions {Prompt = MessageFactory.Text(message)}, cancellationToken);
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            if (puzzleDetails.NumberOfAttemptsLimit.HasValue && puzzleDetails.NumberOfAttemptsLimit.Value > 0)
            {
                var remainCount = puzzleDetails.NumberOfAttemptsLimit.Value - puzzleDetails.NumberOfAttempts;
                var message = $"Количество оставшихся попыток {remainCount} ";
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions {Prompt = MessageFactory.Text(message)}, cancellationToken);
                return await stepContext.ReplaceDialogAsync(puzzleDetails.PuzzleType.ToString(), puzzleDetails,
                    cancellationToken);
            }

            return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
        }
        
        private IActivity GenerateHideKeyboardMarkupForTelegram(IActivity reply)
        {
            var replyMarkup = new
            {
                reply_markup = new
                {
                    hide_keyboard = true
                }
            };

            var channelData = new
            {
                method = "sendMessage",
                parameters = replyMarkup
            };

            reply.ChannelData = JObject.FromObject(channelData);
            return reply;
        }
    }
}