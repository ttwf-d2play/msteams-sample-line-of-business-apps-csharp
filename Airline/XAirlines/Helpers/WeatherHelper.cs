using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Airlines.XAirlines.Helpers
{
    public class WeatherHelper
    {
        public WeatherInfo GetWeatherInfo(string des)
        {
            string appId = "59060e0e58de17a6d41580b85727a2c9";
            string url = string.Format("http://api.openweathermap.org/data/2.5/forecast?q={0}&APPID={1}", des, appId);


            using (WebClient client = new WebClient())
            {
                
                
                    string json = client.DownloadString(url);
               
                WeatherInfo weatherinfo = (new JavaScriptSerializer().Deserialize<WeatherInfo>(json));
                return weatherinfo;
            }
            
        }
    }

    public class WeatherInfo
    {
        public City city { get; set; }
        public List<List> list { get; set; }
    }

    public class City
    {
        public string name { get; set; }
        public string country { get; set; }
    }
    public class Temp
    {
        public double temp { get; set; }
        public double day { get; set; }
        public double min { get; set; }
        public double max { get; set; }
        public double night { get; set; }
    }

    public class Weather
    {
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Main
    {
        public double temp { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double humidity { get; set; }
    }
    public class List
    {
        public Temp temp { get; set; }
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
    }
}
