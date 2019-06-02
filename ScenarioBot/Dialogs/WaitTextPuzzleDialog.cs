using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.BotCommands;
using Core.Domain;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using ScenarioBot.Domain;

namespace ScenarioBot.Dialogs
{
    public class WaitTextPuzzleDialog : TextPuzzleDialog
    {
        private readonly ConcurrentBag<BackgroundNotifyMsg> _backgroundNotifyMsgsStore;

        public WaitTextPuzzleDialog(IList<IBotCommand> botCommands,
            ConcurrentBag<BackgroundNotifyMsg> backgroundNotifyMsgsStore) 
            : base(botCommands, nameof(WaitTextPuzzleDialog) )
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
                var text = $"���������� ����������� ������ �������� ���� ����� {remainMinutesToAnswer} ���";
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