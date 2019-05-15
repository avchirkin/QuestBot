using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Domain;
using CoreBot.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.BotBuilderSamples;

namespace CoreBot.Dialogs
{
    public class WaitTextPuzzleDialog : TextPuzzleDialog
    {
        private readonly ConcurrentBag<BackgroundNotifyMsg> _backgroundNotifyMsgsStore;

        public WaitTextPuzzleDialog(IScenarioService scenarioService, IUserService userService,
            ITeamService teamService, ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
            INotificationMessanger notificationMessanger,
            ConcurrentBag<BackgroundNotifyMsg> backgroundNotifyMsgsStore) 
            : base(scenarioService, userService, teamService, conversationReferences, notificationMessanger, nameof(WaitTextPuzzleDialog))
        {
            _backgroundNotifyMsgsStore = backgroundNotifyMsgsStore;
        }

        protected override async Task<DialogTurnResult> AskDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;

            if (!puzzleDetails.QuestionAskedAt.HasValue)
            {
                // ���� ������ �����, �� ������ ������
                puzzleDetails.SetQuestionAskedAt(DateTime.UtcNow);

                // ������ ���� ����������� ��� ���� �������� ������� � ��������� ����������� ������
                _backgroundNotifyMsgsStore.Add(new BackgroundNotifyMsg()
                {
                    TeamId = puzzleDetails.TeamId,
                    Msg = "�������� ����� �����������, ����� ���������� �����. ������� � �����! :)",
                    WhenByUTC = puzzleDetails.AnswerTimeNoLessThan.AddMinutes(1)
                });

                return await base.AskDialog(stepContext, cancellationToken);
            }

            var remainMinutesToAnswer = puzzleDetails.GetRemainMinutesToAnswer(DateTime.UtcNow);
            if (remainMinutesToAnswer > 0)
            {
                var text = $"���������� ����������� ������ �������� ���� ����� {remainMinutesToAnswer} ���. ��� ������ �����������.";
                return await stepContext.PromptAsync(nameof(TextPrompt),
                    new PromptOptions {Prompt = MessageFactory.Text(text)}, cancellationToken);
            }

            return await stepContext.ContinueDialogAsync(cancellationToken);
        }

        protected override async Task<DialogTurnResult> CheckDialog(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            var puzzleDetails = (PuzzleDetails) stepContext.Options;

            if (puzzleDetails.GetRemainMinutesToAnswer(DateTime.UtcNow) > 0)
            {
                return await stepContext.ReplaceDialogAsync(puzzleDetails.PuzzleType.ToString(), puzzleDetails, cancellationToken);
            }

            return await base.CheckDialog(stepContext, cancellationToken);
        }
    }
}