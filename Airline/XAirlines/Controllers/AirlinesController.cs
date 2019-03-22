using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdaptiveCards.Rendering.Html;
using Airlines.XAirlines.Helpers;
using Airlines.XAirlines.Models;

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
        public ActionResult Portal()
        {
            Portal portal = new Portal();
            AdaptiveCardRenderer renderer = new AdaptiveCardRenderer();
            var html = CardHelper.GetSomeCard();
            RenderedAdaptiveCard renderedCard = renderer.RenderCard(html);
            HtmlTag cardhtml = renderedCard.Html;
            portal.html = cardhtml;
            return View(portal);
        }

    }
}