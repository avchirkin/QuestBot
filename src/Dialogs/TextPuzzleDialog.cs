using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs
{
    public class TextPuzzleDialog : CancelAndHelpDialog
    {
        public TextPuzzleDialog(IScenarioService scenarioService, IUserService userService, ITeamService teamService,
            ConcurrentDictionary<UserId, ConversationReference> conversationReferences, string id = "TextPuzzleDialog") 
            : base(id, scenarioService, userService, teamService, conversationReferences)
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

        protected virtual async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails)stepContext.Options;
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text($"{puzzleDetails.Question}") }, cancellationToken);
        }

        protected virtual async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;
            puzzleDetails.SetAnswer((string) stepContext.Result);

            if (puzzleDetails.IsRight)
            {
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            if (puzzleDetails.NumberOfAttempts >= puzzleDetails.NumberOfAttemptsLimit)
            {
                await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions { Prompt = MessageFactory.Text("� ���������, �� ������������ ��� ������� ������ ���������� �����") }, cancellationToken);
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            // hack! ����� �������� ����� �� �� ������������ ���� ��� WaitTextPuzzleDialog,
            // ��� ���� ���� ����� else branch � ������� ���-�� ������� �� ���� ���������
            if (puzzleDetails.PuzzleType == PuzzleType.WaitTextPuzzleDialog)
            {
                return await stepContext.EndDialogAsync(puzzleDetails, cancellationToken);
            }

            return await stepContext.ReplaceDialogAsync(puzzleDetails.PuzzleType.ToString(), puzzleDetails, cancellationToken);
        }
    }
}