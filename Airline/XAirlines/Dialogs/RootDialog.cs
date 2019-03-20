using Airlines.XAirlines.Common;
using Airlines.XAirlines.Helpers;
using Airlines.XAirlines.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Airlines.XAirlines.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {

        /// <summary>
        /// Called when the dialog is started.
        /// </summary>
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        /// <summary>
        /// Called when a message is received by the dialog
        /// </summary>
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = context.MakeMessage();
            Attachment card = null;
            string message = string.Empty;
            var userName = await GetUserEmailId(activity);
            if (!string.IsNullOrEmpty(activity.Text))
            {
                message = Microsoft.Bot.Connector.Teams.ActivityExtensions.GetTextWithoutMentions(activity).ToLowerInvariant();



                switch (message.Trim())
                {
                    case Constants.NextMonthRoster:
                        card = await CardHelper.GetMonthlyRosterCard();
                        break;
                    case Constants.NextWeekRoster:
                        card = await CardHelper.GetWeeklyRosterCard();
                        break;
                    case Constants.ShowDetailedRoster:
                        card = await CardHelper.GetDetailedRoster(activity);
                        break;
                    case Constants.UpdateCard:
                        card = await CardHelper.GetUpdateScreen();
                        break;

                    default:
                        
                        card = await CardHelper.GetWelcomeScreen(userName);
                        break;
                }
                reply.Attachments.Add(card);
                await context.PostAsync(reply);


            }
            else if (activity.Value != null)
            {

                await HandleActions(context, activity);
                return;
            }
        }

        private async Task HandleActions(IDialogContext context, Activity activity)
        {
            WeatherHelper weather = new WeatherHelper();
            var reply = context.MakeMessage();
            var details = JsonConvert.DeserializeObject<ActionDetails>(activity.Value.ToString());

            CurrencyHelper currency = new CurrencyHelper();
         
                var desLocationInfo = JsonConvert.DeserializeObject<WeatherActionDetails>(activity.Value.ToString());
                //WeatherInfo weatherinfo = weather.GetWeatherInfo(desLocationInfo.City);
          
            

            var type = details.ActionType;
            Attachment card = null;
            switch (type)
            {
                case Constants.ShowDetailedRoster:
                    card=await CardHelper.GetDetailedRoster(activity);
                    break;
                case Constants.NextWeekRoster:
                    card = await CardHelper.GetWeeklyRosterCard();
                    break;
                case Constants.NextMonthRoster:
                    card = await CardHelper.GetMonthlyRosterCard();
                    break;
                case Constants.WeatherCard:
                    //var desLocationInfo = JsonConvert.DeserializeObject<WeatherActionDetails>(activity.Value.ToString());
                    WeatherInfo weatherinfo = weather.GetWeatherInfo(desLocationInfo.City);
                    card = await CardHelper.GetWeatherCard(weatherinfo);
                    break;
                case Constants.CurrencyCard:
                    var desCurrency = JsonConvert.DeserializeObject<WeatherActionDetails>(activity.Value.ToString());
                    CurrencyInfo currencyinfo = currency.GetCurrencyInfo();
                    card = await CardHelper.GetCurrencyCard(currencyinfo, desLocationInfo.City, desLocationInfo.destinationCurrencyCode);
                    break;

            }
            reply.Attachments.Add(card);
            await context.PostAsync(reply);
            return;
        }
        private async Task<string> GetUserEmailId(Activity activity)

        {

            // Fetch the members in the current conversation

            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

            var members = await connector.Conversations.GetConversationMembersAsync(activity.Conversation.Id);

            return members.Where(m => m.Id == activity.From.Id).First().AsTeamsChannelAccount().GivenName;

        }

    }
}
