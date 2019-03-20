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
using Microsoft.Bot.Builder.Dialogs;
using v = Airlines.XAirlines.Helpers;

namespace Airlines.XAirlines.Helpers
{
    public class CardHelper
    {
        public static async Task<Attachment> GetWeeklyRosterCard()
        {
            Common crewdata = new Common();
            Crew crew = crewdata.ReadJson();
            DateTime today = DateTime.Today;
            DateTime weekafter = today.AddDays(6);
            var weekplan = crew.plan.Where(c => c.date >= today && c.date <= weekafter);
            var listCard = new ListCard();
            listCard.content = new Content();
            listCard.content.title = "Here is your next week's roster";
            var list = new List<Item>();
            foreach (var i in weekplan)
            {
                var item = new Item();
                item.id = i.flightDetails.flightStartDate;
                item.type = "resultItem";
                item.icon = i.vacationPlan==true?ApplicationSettings.BaseUrl + "/Resources/vacationicon.png": i.isDayOff==true?ApplicationSettings.BaseUrl + "/Resources/homeicon.png": ApplicationSettings.BaseUrl + "/Resources/flighticon.png";
                item.title = i.vacationPlan==true?i.vacationDate:i.isDayOff==true?i.flightDetails.flightStartDate:i.flightDetails.flightStartDate + "-" + i.flightDetails.flightEndDate;
                item.subtitle = i.vacationPlan==true?i.vacationReason:i.isDayOff==true?"Day Off":i.flightDetails.sourceCode + "-" + i.flightDetails.destinationCode;
                item.tap = i.vacationPlan==true?null:i.isDayOff==true?null:new Tap()
                {
                    type = ActionTypes.MessageBack,
                    title = "Id",
                    value = JsonConvert.SerializeObject(new AirlineActionDetails()
                    { Id = item.id, ActionType = Constants.ShowDetailedRoster })
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
                Title="View Crew Portal",
                Url = new System.Uri(Constants.PortalTabDeeplink)
            });

           

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };


        }
        public static async Task<Attachment> GetWelcomeScreen(string userName)
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
                                Text=$"Hey {userName}! Here is what I can do for you",
                                Size=AdaptiveTextSize.Large
                            },
                            new AdaptiveColumnSet()
                            {
                                Columns=new List<AdaptiveColumn>()
                                {
                                   
                                    new AdaptiveColumn()
                                    {
                                         Width=AdaptiveColumnWidth.Auto,
                                         Items=new List<AdaptiveElement>()
                                         {
                                             new AdaptiveTextBlock(){Text="View Weekly roster",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium, Spacing=AdaptiveSpacing.None, HorizontalAlignment=AdaptiveHorizontalAlignment.Center}
                                         },
                                          SelectAction = new AdaptiveSubmitAction()
                                         {
                                             DataJson=@"{'ActionType':'" + Constants.NextWeekRoster+"'}", Title="Next Week roster"
                                         }
                                    }
                                }
                            },
                            new AdaptiveColumnSet()
                            {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    

                                    new AdaptiveColumn()
                                    {
                                         Width=AdaptiveColumnWidth.Auto,
                                         Items=new List<AdaptiveElement>()
                                         {
                                             new AdaptiveTextBlock(){Text="View Monthly Roster",Color=AdaptiveTextColor.Accent,Size=AdaptiveTextSize.Medium,HorizontalAlignment=AdaptiveHorizontalAlignment.Center,Spacing=AdaptiveSpacing.None }
                                         },
                                          SelectAction = new AdaptiveSubmitAction()
                                         {
                                                DataJson=@"{'ActionType':'" + Constants.NextMonthRoster+"'}", Title="Next Week roster"
                                         }
                                    }
                                }
                            },
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
        public static async Task<Attachment> GetDetailedRoster(Activity activity)
        {
            var details = JsonConvert.DeserializeObject<AirlineActionDetails>(activity.Value.ToString());
            Common crewdata = new Common();
            Crew crew = crewdata.ReadJson();
            var datePlan = crew.plan.Where(c => c.flightDetails.flightStartDate == details.Id).FirstOrDefault();
            var Card = new AdaptiveCard(new AdaptiveSchemaVersion("1.0"))
            {

                Body = new List<AdaptiveElement>()
                     {
                    new AdaptiveColumnSet()
                    {
                        Spacing=AdaptiveSpacing.Small,
                        Columns=new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Items=new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Left,
                                        Spacing=AdaptiveSpacing.Small,
                                        Separator=true,
                                        Weight=AdaptiveTextWeight.Bolder,
                                        Color=AdaptiveTextColor.Attention,
                                        Text="E0370",
                                        MaxLines=1
                                    }
                                },

                            },
                            new AdaptiveColumn()
                            {
                                Items=new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,

                                        Separator=true,

                                        Text="Updated 2 days ago",

                                    }
                                },

                            },
                        }
                    },
                    new AdaptiveColumnSet()
                    {
                        Separator=true,
                        Columns=new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                               Items=new List<AdaptiveElement>()
                               {
                                   new AdaptiveContainer()
                                   {
                                       Items=new List<AdaptiveElement>()
                                       {
                                            new AdaptiveTextBlock()
                                            {
                                               Size=AdaptiveTextSize.Medium,
                                                Weight=AdaptiveTextWeight.Bolder,
                                               Text=datePlan.flightDetails.flightStartDate
                                            },
                                            new AdaptiveTextBlock()
                                            {
                                               Size=AdaptiveTextSize.Small,
                                               Weight=AdaptiveTextWeight.Bolder,
                                               Text=datePlan.flightDetails.flightDepartueTime
                                            },
                                       }
                                   }


                               }
                            },
                            new AdaptiveColumn()
                            {
                                Items=new List<AdaptiveElement>()
                                {
                                          new AdaptiveTextBlock()
                                            {
                                               Size=AdaptiveTextSize.Medium,
                                               HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                                Weight=AdaptiveTextWeight.Bolder,
                                               Text=datePlan.flightDetails.flightEndDate
                                            },
                                            new AdaptiveTextBlock()
                                            {
                                                HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                               Size=AdaptiveTextSize.Small,
                                               Weight=AdaptiveTextWeight.Bolder,
                                               Text=datePlan.flightDetails.flightArrivalTime
                                            },
                                }
                            }
                        }
                    },
                    new AdaptiveColumnSet()
                    {
                      Separator=true,
                      Columns=new List<AdaptiveColumn>()
                      {
                          new AdaptiveColumn()
                          {
                              Items=new List<AdaptiveElement>()
                              {
                                 new AdaptiveTextBlock()
                                 {
                                     HorizontalAlignment=AdaptiveHorizontalAlignment.Center,
                                     Size=AdaptiveTextSize.Medium,
                                     Weight=AdaptiveTextWeight.Bolder,
                                     Text=datePlan.flightDetails.travelDuraion
                                 }
                              }
                          }
                      }
                    },
                    new AdaptiveColumnSet()
                    {
                        Columns=new List<AdaptiveColumn>()
                        
                        {
                            new AdaptiveColumn()
                            {
                                Items=new List<AdaptiveElement>()
                                {
                                    new AdaptiveContainer()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                Size=AdaptiveTextSize.Small,
                                                Weight=AdaptiveTextWeight.Lighter,
                                                Text=datePlan.flightDetails.sourceFlightCode+"-"+datePlan.flightDetails.source

                                            },
                                            new AdaptiveTextBlock()
                                            {
                                                Size=AdaptiveTextSize.ExtraLarge,
                                                Color=AdaptiveTextColor.Accent,
                                                Text=datePlan.flightDetails.sourceCode
                                            }
                                        }
                                    }
                                }
                            },
                            new AdaptiveColumn()
                            {
                                Items=new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                        Size=AdaptiveTextSize.Small,
                                        Weight=AdaptiveTextWeight.Lighter,
                                        Text=datePlan.flightDetails.destinationFlightCode+"-"+datePlan.flightDetails.destination//Need to change
                                    
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                        Size=AdaptiveTextSize.ExtraLarge,
                                       Color=AdaptiveTextColor.Accent,
                                        Text=datePlan.flightDetails.destinationCode

                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                        Size=AdaptiveTextSize.Medium,
                                        Weight=AdaptiveTextWeight.Bolder,
                                        Color=AdaptiveTextColor.Accent,
                                        Text=datePlan.flightDetails.layOVer

                                    },
                                }
                            }
                        }
                        
                    },
                    new AdaptiveColumnSet()
                    {
                        Columns=new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                            {
                                Items=new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        Text="E-Gate Open"
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Text="Block hrs"
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Text="Away from base"
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Text="AC type"
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        Text="Tail No"
                                    },
                                },
                                
                            },
                            new AdaptiveColumn()
                            {
                                Items=new List<AdaptiveElement>()
                                {
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                        Weight=AdaptiveTextWeight.Bolder,
                                        Text=datePlan.flightDetails.gateOpensAt
                                    
                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                        Weight=AdaptiveTextWeight.Bolder,
                                        Text="12h 36 m"//Hard coded

                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                        Weight=AdaptiveTextWeight.Bolder,
                                        Text="05:00,23 Sep"//Hard coded

                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                        Weight=AdaptiveTextWeight.Bolder,
                                        Text=datePlan.flightDetails.acType

                                    },
                                    new AdaptiveTextBlock()
                                    {
                                        HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                        Weight=AdaptiveTextWeight.Bolder,
                                        Text=datePlan.flightDetails.tailNo
                                    },
                                }
                            }
                        },
                        
                    }


                },
                Actions=new List<AdaptiveAction>()
                {
                    new AdaptiveSubmitAction()
                    {
                        Title="Weather Report",
                        Data=new WeatherActionDetails(){City=datePlan.flightDetails.destination,ActionType=Constants.WeatherCard}
                    },
                    new AdaptiveSubmitAction()
                    {
                        Title="Currency Details",
                        Data=new WeatherActionDetails(){City=datePlan.flightDetails.destination,ActionType=Constants.CurrencyCard}
                    }
                }
            };

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };
        }
     
        public static async Task<Attachment> GetUpdateScreen()
        {

            DateTime dateTime;
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
                                      Size=AdaptiveTextSize.Medium,
                                      Weight=AdaptiveTextWeight.Bolder,
                                      Text="Your monthly roster"
                                    },
                            new AdaptiveColumnSet()
                            {
                                Columns=new List<AdaptiveColumn>()
                                {
                                    
                                    new AdaptiveColumn()
                                    {
                                         Width=AdaptiveColumnWidth.Auto,
                                         Items=new List<AdaptiveElement>()
                                         {
                                             
                                             new AdaptiveTextBlock(){Text="New roster for the month of "+DateTime.Now.ToString("MMMM")+" has been released.Please acknowledge to view your new roster",Wrap=true}
                                         },
                                          
                                    },
                                    new AdaptiveColumn()
                                    {
                                         Width=AdaptiveColumnWidth.Auto,
                                         Items=new List<AdaptiveElement>()
                                         {
                                             new AdaptiveImage(){Url=new Uri(ApplicationSettings.BaseUrl + "/Resources/clipboard.PNG"),Id="abc"}
                                         }
                                    },
                                }
                            }
                            
                        }
                    }
                }

            };
            Card.Actions.Add(new AdaptiveSubmitAction()
            {
                Title = "View Crew Portal",
                Data=new ActionDetails() { ActionType=Constants.NextWeekRoster}
            });
            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = Card
            };


        }
        public static async Task<Attachment> GetWeatherCard(string cityName)
        {

            DateTime dateTime;
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
                                      Size=AdaptiveTextSize.Medium,
                                      Weight=AdaptiveTextWeight.Bolder,
                                      Text="Here is the weather report for "+cityName
                                    },
                            new AdaptiveColumnSet()
                            {
                                Columns=new List<AdaptiveColumn>()
                                {

                                    new AdaptiveColumn()
                                    {
                                        
                                         Items=new List<AdaptiveElement>()
                                         {

                                             new AdaptiveTextBlock(){Text="Date of Arrival",HorizontalAlignment=AdaptiveHorizontalAlignment.Left},
                                             new AdaptiveTextBlock(){Text="Sun,24 Nov",Weight=AdaptiveTextWeight.Bolder}
                                         },

                                    },
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveImage()
                                            {
                                                HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                                Url=new Uri(ApplicationSettings.BaseUrl+"/Resources/Sunny-cloudy-weather.png")
                                            }
                                        }
                                    }
                                }
                            },
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
                                                Url=new Uri(ApplicationSettings.BaseUrl+"/Resources/Sunny and cloudy.png")
                                            }
                                        }
                                    },
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                Text="Mostly cloudy",
                                               
                                            }
                                        }
                                    }
                                }
                            },
                            new AdaptiveColumnSet()
                            {
                                Spacing=AdaptiveSpacing.Small,
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                HorizontalAlignment=AdaptiveHorizontalAlignment.Left,
                                                Spacing=AdaptiveSpacing.Small,
                                                Separator=true,
                                                Weight=AdaptiveTextWeight.Lighter,
                                                Text="Minimum",
                                                MaxLines=1
                                            }
                                        }
                                    },
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                                Spacing=AdaptiveSpacing.Small,
                                                Separator=true,
                                                Weight=AdaptiveTextWeight.Lighter,
                                                Text="Maximum",

                                            }
                                        }
                                    },
                                }
                            },
                            new AdaptiveColumnSet()
                            {
                                Separator=true,
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveContainer()
                                            {
                                                Items=new List<AdaptiveElement>()
                                                {
                                                    new AdaptiveTextBlock()
                                                    {
                                                        Size=AdaptiveTextSize.Medium,
                                                        Weight=AdaptiveTextWeight.Bolder,
                                                        Text="26 C"
                                                    }
                                                }
                                            }
                                        }
                                    },
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                                Size=AdaptiveTextSize.Medium,
                                                Weight=AdaptiveTextWeight.Bolder,
                                                Text="32C"
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
        public static async Task<Attachment> GetCurrencyCard()
        {

            DateTime dateTime;
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
                                Spacing=AdaptiveSpacing.Small,
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                HorizontalAlignment=AdaptiveHorizontalAlignment.Left,
                                                Spacing=AdaptiveSpacing.Small,
                                                Separator=true,
                                                Weight=AdaptiveTextWeight.Bolder,
                                                Text="Here are the currency details for Bangalore",

                                            }
                                        }
                                    }
                                }
                            },
                            new AdaptiveColumnSet()
                            {
                                Separator=true,
                                Columns=new List<AdaptiveColumn>()
                                {
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveContainer()
                                            {
                                                Items=new List<AdaptiveElement>()
                                                {
                                                    new AdaptiveTextBlock()
                                                    {
                                                        Size=AdaptiveTextSize.Medium,
                                                        Weight=AdaptiveTextWeight.Lighter,
                                                        Text="1 AED"
                                                    },
                                                    new AdaptiveTextBlock()
                                                    {
                                                        Size=AdaptiveTextSize.Medium,
                                                        Weight=AdaptiveTextWeight.Lighter,
                                                        Text="1 INR"
                                                    },
                                                }
                                            }
                                        }
                                    },
                                    new AdaptiveColumn()
                                    {
                                        Items=new List<AdaptiveElement>()
                                        {
                                            new AdaptiveTextBlock()
                                            {
                                                HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                                Size=AdaptiveTextSize.Medium,
                                                Weight=AdaptiveTextWeight.Bolder,
                                                Text="18.94 INR"
                                            },
                                            new AdaptiveTextBlock()
                                            {
                                                HorizontalAlignment=AdaptiveHorizontalAlignment.Right,
                                                Size=AdaptiveTextSize.Medium,
                                                Weight=AdaptiveTextWeight.Bolder,
                                                Text="0.053 AED"
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
        public static String GetAdaptiveCardJson()
        {
            var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~/Cards/AdaptiveCard.json");
            return File.ReadAllText(path);
        }
    }
}