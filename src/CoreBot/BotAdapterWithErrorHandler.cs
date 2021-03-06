using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Logging;

namespace CoreBot
{
    public class BotAdapterWithErrorHandler : BotFrameworkAdapter
    {
        public BotAdapterWithErrorHandler(ICredentialProvider credentialProvider, 
            ILogger logger,
            IChannelProvider channelProvider = null,
            ConversationState conversationState = null)
            : base(credentialProvider, channelProvider, logger:logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"Exception caught : {exception.Message}  {exception.StackTrace}");
                // Send a catch-all appology to the user.
                await turnContext.SendActivityAsync($"Oops! Sorry, it looks like something went wrong. We've already informed.");
            };
        }
    }
    
    public class BotHttpAdapterWithErrorHandler : BotFrameworkHttpAdapter
    {
        public BotHttpAdapterWithErrorHandler(ICredentialProvider credentialProvider, 
            ILogger<BotFrameworkHttpAdapter> logger,
            IChannelProvider channelProvider = null,
            ConversationState conversationState = null)
            : base(credentialProvider, channelProvider, logger:logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Log any leaked exception from the application.
                logger.LogError(exception, $"Exception caught : {exception.Message}  {exception.StackTrace}");
                // Send a catch-all appology to the user.
                await turnContext.SendActivityAsync($"Oops! Sorry, it looks like something went wrong. We've already informed.");

                if (conversationState != null)
                    try
                    {
                        // Delete the conversationState for the current conversation to prevent the
                        // bot from getting stuck in a error-loop caused by being in a bad state.
                        // ConversationState should be thought of as similar to "cookie-state" in a Web pages.
                        await conversationState.DeleteAsync(turnContext);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e,$"Exception caught on attempting to Delete ConversationState : {e.Message}");
                    }
            };
        }
    }
}