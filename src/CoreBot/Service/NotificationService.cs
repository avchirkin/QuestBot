﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Schema;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain;
using Core.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScenarioBot.Service;

namespace CoreBot
{
    public class NotificationService : INotificationService
    {
        private readonly IAdapterIntegration _adapter;
        private readonly string _botAppId;
        public NotificationService(string botAppId, IAdapterIntegration adapter)
        {
            _botAppId = botAppId ?? throw new ArgumentNullException(nameof(botAppId));
            _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
        }

        public async Task SendMessage(string message, ConversationReference conversationReference, CancellationToken cancellationToken)
        {
            var teamMessage = new TeamMessage()
            {
                Message = message
            };
            await teamMessage.SendMessage(_adapter, _botAppId, conversationReference, cancellationToken);
        }

        public async Task SendMessageInBackground(BackgroundNotifyMsg msg)
        {
            return;
        }


        class TeamMessage
        {
            public string Message { get; set; }

            public async Task SendMessage(IAdapterIntegration adapter, string botAppId, ConversationReference conversationReference, CancellationToken cancellationToken)
            {
                await adapter.ContinueConversationAsync(botAppId, conversationReference, SendMessageAsync, default(CancellationToken));
            }

            private async Task SendMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
            {
                await turnContext.SendActivityAsync(Message);
            }

        }
    }
    
    static class UserServiceExtensions
    {
        public static async Task<User> GetOrCreateUser(this IUserService userService, ITurnContext context)
        {
            var user = await userService.GetByAsync(context.Activity.ChannelId, context.Activity.From.Id);
            if (user == null)
            {
                user = new User(context.Activity.ChannelId, context.Activity.From.Id)
                {
                    Name = GetUserName(context.Activity.From, context.Activity.ChannelData),
                    ChannelData = context.Activity.ChannelData != null ? context.Activity.ChannelData.ToString() : string.Empty,
                    ConversationData = GetConversationData(context.Activity.GetConversationReference())
                };
                await userService.InsertOrMergeAsync(user);
            }
            return user;
        }

        public static async Task<User> AddOrUpdateConversation(this IUserService userService, ITurnContext context, ConversationReference conversationReference)
        {
            var user = await userService.GetByAsync(context.Activity.ChannelId, context.Activity.From.Id);
            if (user == null)
            {
                user = new User(context.Activity.ChannelId, context.Activity.From.Id)
                {
                    Name = GetUserName(context.Activity.From, context.Activity.ChannelData),
                    ChannelData = context.Activity.ChannelData != null ? context.Activity.ChannelData.ToString() : string.Empty
                };
            }
            user.ConversationData = GetConversationData(conversationReference);
            await userService.InsertOrMergeAsync(user);
            return user;
        }

        private static string GetConversationData(ConversationReference conversationReference)
        {
            return JsonConvert.SerializeObject(conversationReference);
        }

//        public static async Task<User> ValidateUser(this IUserService userService, UserId userId)
//        {
//            var user = await userService.GetByAsync(userId.ChannelId, userId.Id);
//            if (user == null)
//                throw new UserNotFoundException(userId);
//            return user;
//        }

        private static string GetUserName(ChannelAccount account, object channelData)
        {
            var result = account.Name;
            var personInfo = GetPersonInfo(channelData);
            var fullName = personInfo.firstName;
            if (!String.IsNullOrEmpty(fullName) && !String.IsNullOrEmpty(personInfo.lastName))
                fullName += " ";
            fullName += personInfo.lastName;
            if (!String.IsNullOrEmpty(fullName))
                if (String.IsNullOrEmpty(result))
                    result = fullName;
                else
                    result += " (" + fullName + ")";
            return result;

        }

        private static (string firstName, string lastName) GetPersonInfo(object channelData)
        {
            (string firstName, string lastName) result = (null, null);
            try
            {
                if (channelData == null)
                    return result;
                var parseChannelData = JObject.Parse(channelData.ToString());
                result.firstName = parseChannelData != null ? parseChannelData.SelectToken("message.from.first_name")?.Value<string>() : null;
                result.lastName = parseChannelData != null ? parseChannelData.SelectToken("message.from.last_name")?.Value<string>() : null;
                return result;
            }
            catch(Exception)
            {
                return result;
            }
        }
    }
    
    static class TurnContextExtensions
    {
        public static async Task<ResourceResponse> SendMessageAsync(ITurnContext turnContext, string message, CancellationToken cancellationToken)
        {
            if (turnContext == null)
            {
                throw new System.ArgumentNullException(nameof(turnContext));
            }

            var activity = MessageFactory.Text(message, null, InputHints.IgnoringInput);
            return await turnContext.SendActivityAsync(activity, cancellationToken);
        }
    }
    
    //using CoreBot.Domain;
//using CoreBot.Service;
//using Microsoft.Bot.Builder;
//using Microsoft.Bot.Schema;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//
//namespace CoreBot
//{
//    static class TeamUtils
//    {
//
//        public static async Task SendTeamMessage(ITeamService teamService,
//            ITurnContext turnContext,
//            INotificationMessanger messenger,
//            string teamId,
//            string message,
//            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
//            CancellationToken cancellationToken,
//            bool sendMe = true)
//        {
//            var users = await teamService.GetTeamMembers(teamId);
//            var teamConversations = conversationReferences.ToArray();
//            if (sendMe)
//                await TurnContextExtensions.SendMessageAsync(turnContext, message, cancellationToken);
//            var excludeUserId = new UserId(turnContext.Activity.ChannelId, turnContext.Activity.From.Id);
//            foreach (var reference in teamConversations.Where(i => users.Contains(i.Key)))
//            {
//                if (reference.Key.Equals(excludeUserId))
//                    continue;
//                await messenger.SendMessage(message, reference.Value, cancellationToken);
//            }
//        }
//
//        public static void SendTeamMessage(ITeamService teamService,
//            INotificationMessanger messenger,
//            string teamId,
//            string message,
//            ConcurrentDictionary<UserId, ConversationReference> conversationReferences,
//            CancellationToken cancellationToken
//            )
//        {
//            var users = teamService.GetTeamMembers(teamId).GetAwaiter().GetResult();
//            var teamConversations = conversationReferences.ToArray();
//            foreach (var reference in teamConversations.Where(i => users.Contains(i.Key)))
//            {
//                messenger.SendMessage(message, reference.Value, cancellationToken).GetAwaiter().GetResult();
//            }
//        }
//    }
//}

}
