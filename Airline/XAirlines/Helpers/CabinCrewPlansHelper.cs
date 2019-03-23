using Airlines.XAirlines.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Airlines.XAirlines.Helpers
{
    public class CabinCrewPlansHelper
    {
        public static async Task<Crew> ReadJson(string userEmailId)
        {
            string location = ApplicationSettings.BaseUrl;
            userEmailId = userEmailId.ToLower();

            var value = userEmailId.First();
            if (userEmailId.Contains("v-")) // Check for v- emailIds.
                value = userEmailId.Skip(2).First();


            int fileNumber = (value % 5) + 1;
            fileNumber = 1; // No flight detail availabe

            string file = System.Web.Hosting.HostingEnvironment.MapPath(@"~\TestData\"+ fileNumber + ".json");
            string data = string.Empty;
            if (File.Exists(file))
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    data = await reader.ReadToEndAsync();
                    Crew crews = (new JavaScriptSerializer().Deserialize<Crew>(data));
                    return crews;                    
                }
            }
            else
                return null;
        }

        public static async Task<List<Plan>> WeeksPlan(string userEmailId)
        {
            Crew crew = await CabinCrewPlansHelper.ReadJson(userEmailId);
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
