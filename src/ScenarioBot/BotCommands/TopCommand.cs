using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Core.BotCommands;
using Core.Domain;
using Microsoft.Bot.Builder.Dialogs;
using ScenarioBot.Service;

namespace ScenarioBot.BotCommands
{
    public class TopCommand : IBotCommand
    {
        private readonly IUserService _userService;
        private const string TopCommandPrefix = "top";
        private const int DefaultUserCount = 10;
        private const int MaxAllowedUserCountLength = 2;

        public TopCommand(IUserService userService)
        {
            _userService = userService;
        }

        public bool IsApplicable(string message, UserId userId)
        {
            message = message.Trim();
            return message.StartsWith(TopCommandPrefix, StringComparison.InvariantCultureIgnoreCase) &&
                   message.Length <= TopCommandPrefix.Length + MaxAllowedUserCountLength &&
                   message.Substring(TopCommandPrefix.Length).All(char.IsDigit);
        }

        public bool Validate(UserId userId)
        {
            return true;
        }

        public async Task<DialogTurnResult> ExecuteAsync(DialogContext dialogContext, UserId userId,
            CancellationToken cancellationToken)
        {
            var userCount = GetUserCount(dialogContext.Context.Activity.Text);
            var userWeights = await _userService.CalcUserWeightsAsync(userCount);

            var sb = new StringBuilder();
            sb.Append($"Top {userCount} \r\n");
            
            foreach (var userWeight in userWeights) sb.Append($"{userWeight.Key} - {userWeight.Value} \r\n");

            await dialogContext.Context.SendActivityAsync(sb.ToString(), cancellationToken: cancellationToken);

            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        public IList<ComponentDialog> GetComponentDialogs()
        {
            return new List<ComponentDialog>();
        }
        
        public static int GetUserCount(string commandText)
        {
            if (string.IsNullOrWhiteSpace(commandText) || commandText.Length < TopCommandPrefix.Length)
                return DefaultUserCount;
            
            //здесь нужно нормализовать строку, потому текст команды приходит /top<номер>,  а в метод IsApplicable текст команды приходит без /
            var normalizedCommand = commandText.StartsWith("/") 
                ? commandText.Substring(1)
                : commandText;
            
            var userCountString = normalizedCommand.Substring(TopCommandPrefix.Length);
            return int.TryParse(userCountString, out var count) ? count : DefaultUserCount;
        }
    }
}