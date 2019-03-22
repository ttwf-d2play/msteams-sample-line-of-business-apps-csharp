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


        public static Crew ReadJson()
        {
            int filenumber = 0;
            string location = ApplicationSettings.BaseUrl;
            string userName = (Environment.UserName).ToLower();
            if(userName[0] == 'v')
            {
                if (userName[3] >= 'a' && userName[3] <= 'e') filenumber = 1;
                if (userName[3] >= 'f' && userName[3] <= 'j') filenumber = 2;
                if (userName[3] >= 'k' && userName[3] <= 'o') filenumber = 3;
                if (userName[3] >= 'p' && userName[3] <= 't') filenumber = 4;
                if (userName[3] >= 'u' && userName[3] <= 'z') filenumber = 5;
            }
            else
            {
                if (userName[0] >= 'a' && userName[0] <= 'e') filenumber = 1;
                if (userName[0] >= 'f' && userName[0] <= 'j') filenumber = 2;
                if (userName[0] >= 'k' && userName[0] <= 'o') filenumber = 3;
                if (userName[0] >= 'p' && userName[0] <= 't') filenumber = 4;
                if (userName[0] >= 'u' && userName[0] <= 'z') filenumber = 5;

            }
            
            string file = System.Web.Hosting.HostingEnvironment.MapPath(@"~\TestData\"+ filenumber +".json");
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

        public static List<Plan> WeeksPlan()
        {
            Crew crew = Common.ReadJson();
            DateTime today = DateTime.Today;
            DateTime weekafter = today.AddDays(6);
            List<Plan> weekplan = crew.plan.Where(c => c.date >= today && c.date <= weekafter).ToList();
            return weekplan;

        }

        public List<DateTime> OneMonthsDates()
        {
            DateTime today = DateTime.Now;
            List<DateTime> oneMonthDates = new List<DateTime>();
            oneMonthDates.Add(today);
            for (int i = 1; i < 30; i++)
            {
                oneMonthDates.Add(today.AddDays(i));
            }
            return oneMonthDates;
        }

    }



}
