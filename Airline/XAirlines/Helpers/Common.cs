using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Airlines.XAirlines.Models;
using System.Web.Script.Serialization;

namespace Airlines.XAirlines.Helpers
{
    public class Common
    {
        private static int GenrateRandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public Crew ReadJson()
        {
            int filenumber = GenrateRandomNumber(1, 10);
            string location = ApplicationSettings.BaseUrl;
            
            string file = System.Web.Hosting.HostingEnvironment.MapPath(@"~\TestData\1.json");
            string data = string.Empty;
            if (File.Exists(file))
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    data = reader.ReadToEnd();
                    Crew crews = (new JavaScriptSerializer().Deserialize<Crew>(data));
                    return crews;
                }
            }
            else
                return null;
        }
    }
}
        //public Crew GetWeeksData()
        //{
        //    Crew crew = ReadJson();

        //    DateTime today = DateTime.Today;

        //    DateTime weekafter = today.AddDays(6);

        //    Crew weekplan = (Crew)crew.plan.Where(c => c.date >= today && c.date <= weekafter);

        //    return weekplan;
        //}

        //public static Crew GetDataByDate(DateTime date)
        //{
        //    Crew crew = ReadJson();

        //    DateTime today = DateTime.Today;

        //    DateTime weekafter = today.AddDays(6);

        //    Crew datePlan = (Crew)crew.plan.Where(c => c.date == date);

        //    return datePlan;
        