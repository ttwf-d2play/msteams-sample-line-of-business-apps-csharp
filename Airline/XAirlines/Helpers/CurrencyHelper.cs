using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Airlines.XAirlines.Helpers
{
    public class CurrencyHelper
    {
        public void GetCurrencyInfo()
        {
           
            string apiKey = "29d0ff0f89f41d3bdd19f6c25ea4b1c4";

            string url = string.Format("http://www.apilayer.net/api/live?access_key=29d0ff0f89f41d3bdd19f6c25ea4b1c4");

            using (WebClient client = new WebClient())
            {
                string json = client.DownloadString(url);
                CurrencyConverter curr = (new JavaScriptSerializer().Deserialize<CurrencyConverter>(json));
            }
        }
    }
        public class CurrencyConverter
        {

            public bool success { get; set; }
            public string timestamp { get; set; }
            public string source { get; set; }
            public Dictionary<string, double> quotes { get; set; }
        }
    
}