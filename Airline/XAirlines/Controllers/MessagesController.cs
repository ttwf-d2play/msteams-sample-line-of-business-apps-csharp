using Airlines.XAirlines.Common;
using Airlines.XAirlines.Dialogs;
using Airlines.XAirlines.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Microsoft.Bot.Connector.Teams.Models;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Airlines.XAirlines.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    await Conversation.SendAsync(activity, () => new RootDialog());
                    break;

                case ActivityTypes.Invoke:
                    return await HandleInvokeActivity(activity);

                case ActivityTypes.ConversationUpdate:
                    await HandleConversationUpdate(activity);
                    break;


            }
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Handle an invoke activity.
        /// </summary>
        private async Task<HttpResponseMessage> HandleInvokeActivity(Activity activity)
        {
            var activityValue = activity.Value.ToString();

            switch (activity.Name)
            {
                case "signin/verifyState":
                    await Conversation.SendAsync(activity, () => new RootDialog());
                    break;

                case "composeExtension/query":
                // Handle fetching task module content

                case "task/fetch":
                // Handle fetching task module content

                case "task/submit":
                    // Handle submission of task module info
                    // Run this on a task so that 

                    break;
            }
            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        /// <summary>
        /// Handle request to fetch task module content.
        /// </summary>
        private static async Task HandleConversationUpdate(Activity message)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
            var channelData = message.GetChannelData<TeamsChannelData>();

            switch (channelData.EventType)
            {
                case "teamMemberAdded":
                    // Team member was added (user or bot)
                    for (int i = 0; i < message.MembersAdded.Count; i++)
                    {
                       if (message.MembersAdded[i].Id == message.Recipient.Id)
                        {
                            // Bot is added. Let's send welcome message.
                            message.Text = "hi";
                            await Conversation.SendAsync(message, () => new RootDialog());
                            break;

                        }
                        else
                        {
                            try
                            {
                                var userId = message.MembersAdded[i].Id;
                                var connectorClient = new ConnectorClient(new Uri(message.ServiceUrl));
                                var parameters = new ConversationParameters
                                {
                                    Members = new ChannelAccount[] { new ChannelAccount(userId) },
                                    ChannelData = new TeamsChannelData
                                    {
                                        Tenant = channelData.Tenant,
                                        Notification = new NotificationInfo() { Alert = true }
                                    }
                                };

                                var conversationResource = await connectorClient.Conversations.CreateConversationAsync(parameters);
                                var replyMessage = Activity.CreateMessageActivity();
                                replyMessage.ChannelData = new TeamsChannelData() { Notification = new NotificationInfo(true) };
                                replyMessage.Conversation = new ConversationAccount(id: conversationResource.Id.ToString());

                                var name = message.MembersAdded[i].Name;

                                if (name != null)
                                {
                                    name = name.Split(' ').First();
                                }
                                var card = CardHelper.GetWelcomeScreen(name);
                                replyMessage.Attachments.Add(card);

                                await connectorClient.Conversations.SendToConversationAsync((Activity)replyMessage);

                            }
                            catch (Exception ex)
                            {
                                ErrorLogService.LogError(ex);
                            }
                        }
                    }
                    break;
                case "teamMemberRemoved":
                    // Add team & channel details 
                    if (message.MembersRemoved.Any(m => m.Id.Contains(message.Recipient.Id)))
                    {
                        // Bot was removed from a team: remove entry for the team in the database

                    }
                    else
                    {
                        // Member was removed from a team: update the team member  count

                    }
                    break;
                // Update the team and channel info in the database when the team is rename or when channel are added/removed/renamed
                case "teamRenamed":
                    // Rename team & channel details 

                    break;

                case "channelCreated":

                    break;
                case "channelRenamed":

                    break;

                case "channelDeleted":

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Handle messageReaction events, which indicate user liked/unliked a message sent by the bot.


    }

}