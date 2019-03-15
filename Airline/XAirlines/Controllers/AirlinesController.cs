using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Airlines.XAirlines.Controllers
{
    public class AirlinesController : Controller
    {
        [Route("index")]
        public ActionResult Index()
        {
            return View();
        }
    }
}