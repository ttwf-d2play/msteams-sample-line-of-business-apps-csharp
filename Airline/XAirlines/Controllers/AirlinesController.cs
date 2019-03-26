using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdaptiveCards.Rendering.Html;
using Airlines.XAirlines.Helpers;
using Airlines.XAirlines.Models;
using Airlines.XAirlines.ViewModels;
using static Airlines.XAirlines.Helpers.WeatherHelper;
using System.Threading.Tasks;
using AdaptiveCards;

namespace Airlines.XAirlines.Controllers
{
    public class AirlinesController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
        [Route("portal")]
        public async Task<ActionResult> Portal(string userEmailId)
        {

            ////TODO:  Fetch Cabin crew data.
            if (string.IsNullOrEmpty(userEmailId))
            {
                return HttpNotFound();
            }

            var cabinCrewData = await CabinCrewPlansHelper.MonthsPlan(userEmailId);
            if(cabinCrewData == null)
                return HttpNotFound();
            List<Duty> duties = new List<Duty>();
            var colour = new[] { "pink", "org", "blue" };
            foreach (var item in cabinCrewData)
            {
              duties.Add(item.vacationPlan==true?new Duty() {Date=item.vacationDate,isDayOff=item.isDayOff,vacationPlan=item.vacationPlan,Details=new Details() {DisplayText="AL",Colour=colour[1] } }:item.isDayOff==true?new ViewModels.Duty() { Date=item.date, isDayOff = item.isDayOff,vacationPlan = item.vacationPlan,Details=new Details() {DisplayText="OFF",Colour=colour[0]} } :new Duty(){Date = Convert.ToDateTime(item.flightDetails.flightStartDate), isDayOff = item.isDayOff,vacationPlan = item.vacationPlan, Details = new Details() { DisplayText = item.flightDetails.sourceCode, Colour = colour[2]} });
            }

            // List<Duty> duties = new List<Duty>();
            //var destinations = new[] { "AL", "OFF", "DEL", "BLR", "OFF", "DEL", "HYD", "OFF", "DEL", "BOM", "DEL", "BLR", "OFF", "DEL", "HYD", };
            //var colour = new[] { "pink", "org", "blue" };

            //for (int i = 0; i < 15; i++)
            //{
            // duties.Add(new Duty() { Date = DateTime.Today.AddDays(i * 2), Details = new Details() { DisplayText = destinations[i], Colour = colour[i % 3] } });
            //}

            //duties[5].Details.Event2 = "BLR";

           
                PortalViewModel viewModel = new PortalViewModel(DateTime.Today, 30, 2, duties);
            
           viewModel.UserEmailId = userEmailId;
            
            return View(viewModel);
        }

        [Route("duty")]
        public async Task<ActionResult> Duty(string code,string userEmailId)
        {
            //Portal portal = new Portal();
            //AdaptiveCardRenderer renderer = new AdaptiveCardRenderer();

            //WeatherHelper weather = new WeatherHelper();
            //WeatherInfo weatherinfo = weather.GetWeatherInfo("Moscow");
            //var card = await CardHelper.GetWeatherCard(weatherinfo, DateTime.Now.Date); // change this date
            //RenderedAdaptiveCard renderedCard = renderer.RenderCard(card.Content as AdaptiveCard);
            //HtmlTag cardhtml = renderedCard.Html;
            //portal.html = cardhtml;
            //return View("AdaptiveCardRenderer", portal);
            Portal portal = new Portal();
            AdaptiveCardRenderer renderer = new AdaptiveCardRenderer();
            var card = await CardHelper.GetMyDetailedCard(code, userEmailId);
            RenderedAdaptiveCard renderedCard = renderer.RenderCard(card.Content as AdaptiveCard);
            HtmlTag cardhtml = renderedCard.Html;
            portal.html = cardhtml;
            return View("AdaptiveCardRenderer", portal);
        }

        [Route("Weather")]
        public async Task<ActionResult> Weather(string city)
        {
            Portal portal = new Portal();
            AdaptiveCardRenderer renderer = new AdaptiveCardRenderer();

            
            WeatherInfo weatherinfo = WeatherHelper.GetWeatherInfo(city);
            var card = await CardHelper.GetWeatherCard(weatherinfo, DateTime.Now.Date); // change this date
            RenderedAdaptiveCard renderedCard = renderer.RenderCard(card.Content as AdaptiveCard);
            HtmlTag cardhtml = renderedCard.Html;
            portal.html = cardhtml;
            return View("AdaptiveCardRenderer", portal);
        }

        //[Route("Currency")]
        //public ActionResult Currency(string source, string destination)
        //{
        //    Portal portal = new Portal();
        //    AdaptiveCardRenderer renderer = new AdaptiveCardRenderer();
        //    var html = CardHelper.GetMyDetailedCard();
        //    RenderedAdaptiveCard renderedCard = renderer.RenderCard(html);
        //    HtmlTag cardhtml = renderedCard.Html;
        //    portal.html = cardhtml;
        //    return View("AdaptiveCardRenderer", portal);
        //}

    }
}