using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Airlines.XAirlines.Helpers
{
    public class WeatherHelper
    {
        public WeatherInfo GetWeatherInfo(string des)
        {
            string appId = "619590f1e4a82a6ed18ee9b109bb9c14";
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?q={0}&APPID=619590f1e4a82a6ed18ee9b109bb9c14", des);
            //string url = string.Format("http://www.apilayer.net/api/live?access_key=29d0ff0f89f41d3bdd19f6c25ea4b1c4");

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
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public int visibility { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }
    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
    }
    public class Wind
    {
        public float speed { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }
    public class Sys
    {
        public int type { get; set; }
        public int id { get; set; }
        public float message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    //public class WeatherInfo
    //{
    //    //public double coord { get; set; }
    //    public Weather weather { get; set; }
    //    public string base1 { get; set; }
    //    public Tempratures main { get; set; }
    //}

    //public class Weather
    //{
    //    public int id { get; set; }
    //    public string main { get; set; }
    //    public string description { get; set; }        
    //}
    //public class Tempratures
    //{
    //    public double temprature { get; set; }
    //    public double pressure { get; set; }
    //    public double humidity { get; set; }
    //    public double temp_min { get; set; }
    //    public double temp_max { get; set; }
    //}


    //public class WeatherInfo
    //{
    //    public City city { get; set; }
    //    public List<List> list { get; set; }
    //}

    //public class City
    //{
    //    public string name { get; set; }
    //    public string country { get; set; }
    //}
    //public class Temp
    //{
    //    public double temp { get; set; }
    //    public double day { get; set; }
    //    public double min { get; set; }
    //    public double max { get; set; }
    //    public double night { get; set; }
    //}

    //public class Weather
    //{
    //    public string description { get; set; }
    //    public string icon { get; set; }
    //}

    //public class Main
    //{
    //    public double temp { get; set; }
    //    public double temp_min { get; set; }
    //    public double temp_max { get; set; }
    //    public double humidity { get; set; }
    //}
    //public class List
    //{
    //    public Temp temp { get; set; }
    //    public Main main { get; set; }
    //    public List<Weather> weather { get; set; }
    //}
}
