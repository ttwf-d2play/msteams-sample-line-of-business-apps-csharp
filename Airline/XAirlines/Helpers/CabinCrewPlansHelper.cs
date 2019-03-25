﻿using Airlines.XAirlines.Models;
using Newtonsoft.Json;
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
            //fileNumber = 1; // No flight detail availabe

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
        public static async Task<List<Plan>> MonthsPlan(string userEmailId)
        {
            Crew crew = await CabinCrewPlansHelper.ReadJson(userEmailId);
            DateTime today = DateTime.Today;
            DateTime monthafter = today.AddDays(30);
            List<Plan> weekplan = crew.plan.Where(c => c.date >= today && c.date <= monthafter).ToList();
            return weekplan;

        }
        public static void UpdateMockData()
        {
            if (DateTime.Now.Day == 1) //check first login of that day
            {
                for (int i = 1; i <= 5; i++)
                {
                    string file = @"C:\Users\v-abjodh\Desktop\Teams\AirlinesJson\" + i + ".json";
                    string data = string.Empty;
                    Crew crewObject;
                    if (File.Exists(file))
                    {
                        using (StreamReader reader = new StreamReader(file))
                        {
                            data = reader.ReadToEnd();
                            crewObject = (new JavaScriptSerializer().Deserialize<Crew>(data));
                        }

                        //for (int i = 0; i < crewObject.plan.Count; i++)
                        //{
                           // crewObject.plan[i].date = crewObject.plan[i].date.AddMonths(1);
                            //CultureInfo provider = CultureInfo.InvariantCulture;
                            //string lastDate = crewObject.plan[i].lastUpdated;

                            //DateTime lastUpdatedDate = DateTime.ParseExact(lastDate, "yyyy/mm/dd", provider);
                            //crewObject.plan[i].lastUpdated = lastUpdatedDate.AddMonths(1).ToString();

                            //DateTime startDate = Convert.ToDateTime(crewObject.plan[i].flightDetails.flightStartDate);
                            //crewObject.plan[i].flightDetails.flightStartDate = startDate.AddMonths(1).ToString();

                            //DateTime endDate = Convert.ToDateTime(crewObject.plan[i].flightDetails.flightEndDate);
                            //crewObject.plan[i].flightDetails.flightStartDate = endDate.AddMonths(1).ToString();

                       // }
                        string json = JsonConvert.SerializeObject(crewObject);
                        File.WriteAllText(file, json);
                    }
                }
                
            }
        }

        public static  List<DateTime> OneMonthsDates()
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
