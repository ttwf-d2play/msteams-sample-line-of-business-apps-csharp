using AdaptiveCards.Rendering.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airlines.XAirlines.Models
{
    public class ActionDetails:WelcomeActionDetails
    {
        public string ActionType { get; set; }
    }
    public class AirlineActionDetails : ActionDetails
    {
        public string Id { get; set; }
    }
    public class WelcomeActionDetails
    {
        public string Text { get; set; }
    }
    public class WeatherActionDetails:ActionDetails
    {
        public string City { get; set; }
        public string Date { get; set; }
        public string sourceCurrencyCode { get; set; }
        public string destinationCurrencyCode { get; set; }
    }
    public class Portal
    {
        public HtmlTag html { get; set; }
    }
}