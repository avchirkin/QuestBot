using System.Threading;
using System.Threading.Tasks;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace CoreBot.Dialogs
{
    public class ScenarioListDialog : ComponentDialog
    {
        private readonly IScenarioService _scenarioService;
        private readonly IUserService _userService;

        public ScenarioListDialog(IScenarioService scenarioService, IUserService userService) : base(nameof(ScenarioListDialog))
        {
            _scenarioService = scenarioService;
            _userService = userService;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)) { Style = ListStyle.SuggestedAction });

            var waterfallStep = new WaterfallStep[]
            {
                ShowChoiceDialog,
                AnswerToChoiceDialog
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallStep));
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowChoiceDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var teamId = (string)stepContext.Options;

            return await stepContext.PromptAsync(nameof(ChoicePrompt),
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("����������, �������� ��������:"),
                    RetryPrompt = MessageFactory.Text("����������, �������� �������� :"),
                    Choices = ChoiceFactory.ToChoices(_scenarioService.GetAvailableScenario(teamId)),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> AnswerToChoiceDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var scenarioId = ((FoundChoice)stepContext.Result).Value;
            var teamId = (string)stepContext.Options;

            var scenarioDetails = _userService.GetLastScenarioDetailsExceptGameOver(teamId);

            if (scenarioDetails == null || scenarioDetails.ScenarioId != scenarioId)
            {
                scenarioDetails = new ScenarioDetails()
                {
                    ScenarioId = scenarioId,
                    TeamId = teamId
                };
            }

            var reply = stepContext.Context.Activity.CreateReply($"��������� ��������: {scenarioId}");
            GenerateHideKeybordMarkupForTelegram(reply);
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            return await stepContext.BeginDialogAsync(nameof(ScenarioDialog), scenarioDetails, cancellationToken);
        }

        

        private void GenerateHideKeybordMarkupForTelegram(IActivity reply)
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
                parameters = replyMarkup,
            };

            reply.ChannelData = JObject.FromObject(channelData);
        }
    }

}