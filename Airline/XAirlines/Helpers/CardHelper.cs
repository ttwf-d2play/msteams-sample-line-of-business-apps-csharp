using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AdaptiveCards;
using Airlines.XAirlines.Models;
using Microsoft.Bot.Connector;
using Bogus;
using Newtonsoft.Json;
using Airlines.XAirlines.Common;
using System.IO;

namespace Airlines.XAirlines.Helpers
{
    public class CardHelper
    {
        private string botId = ConfigurationManager.AppSettings["MicrosoftAppId"];
        private const string TabEntityID = "statictab";
        public static string Text = "Matt Heigder";
        public static string Role = "UX Designer";
        static string Uri = "https://pbs.twimg.com/profile_images/3647943215/d7f12830b3c17a5a9e4afcc370e3a37e_400x400.jpeg";
        public static async Task<Attachment> GetWeeklyRosterCard()
        {


            var listCard = new ListCard();
            listCard.content = new Content();
            listCard.content.title = "Here is your next week's roster";
            var list = new List<Item>();
            for (int i = 0; i < 8; i++)
            {
                var item = new Item();
                item.id = i.ToString();
                item.type = "resultItem";
                item.icon = "http://lorempixel.com/640/480?rand=" + DateTime.Now.Ticks.ToString();
                item.title = "Test Roster" + i;
                item.subtitle = "test roster subtitle" + i;
                item.tap = new Tap()
                {
                    type = ActionTypes.MessageBack,
                    title = "Id",
                    value = JsonConvert.SerializeObject(new AirlineActionDetails()
                    {ActionType = Constants.ShowDetailedRoster })
                };
                list.Add(item);
            }

            listCard.content.items = list.ToArray();

            Attachment attachment = new Attachment();
            attachment.ContentType = listCard.contentType;
            attachment.Content = listCard.content;
            return attachment;

        }
        public static async Task<Attachment> GetMonthlyRosterCard()
        {


            var Card = new AdaptiveCard(new AdaptiveSchemaVersion("1.0"))
            {

                Body = new List<AdaptiveElement>()
                     {

                    new AdaptiveContainer()
                    {
                        Items=new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock()
                            {
                                Text="please access the portal tab to view the monthly roster",
                                Wrap=true
                            }
                        }
                    }
                },
            };
            Card.Actions.Add(new AdaptiveOpenUrlAction()
            {
                Title="View Crew Portal"
            });

           

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };


        }
        public static async Task<Attachment> GetWelcomeScreen()
        {


            var Card = new AdaptiveCard(new AdaptiveSchemaVersion("1.0"))
            {

                Body = new List<AdaptiveElement>()
                     {

                    new AdaptiveContainer()
                    {
                        Items=new List<AdaptiveElement>()
                        {
                            new AdaptiveColumnSet()
                            {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveImage()
                                            {
                                                Style=AdaptiveImageStyle.Person,

                                                Url=new Uri(Uri),
                                                Size=AdaptiveImageSize.Small,

                                            }
                                        },
                                        Width="auto"

                                    },
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                Weight=AdaptiveTextWeight.Bolder,
                                                Text=Text,
                                                Wrap=true,

                                            },
                                            new AdaptiveTextBlock()
                                            {
                                                Spacing=AdaptiveSpacing.None,
                                                Text=Role,
                                                IsSubtle=true,
                                                Wrap=true

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            };

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };


        }
        public static async Task<Attachment> GetDetailedRoster()
        {
            // Parse the JSON 
            AdaptiveCardParseResult result = AdaptiveCard.FromJson(GetAdaptiveCardJson());

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = result.Card

            };
        }

        public static String GetAdaptiveCardJson()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Cards/AdaptiveCard.json");
            return File.ReadAllText(path);
        }
    }
}