using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading.Tasks;


namespace Airlines.XAirlines.Helpers
{
    public class CurrencyHelper
    {
        public static CurrencyInfo GetCurrencyInfo()
        {
            string url = string.Format("http://www.apilayer.net/api/live?access_key=29d0ff0f89f41d3bdd19f6c25ea4b1c4");
            string backupDataLocation = System.Web.Hosting.HostingEnvironment.MapPath(@"~\TestData\CurrencyBackupMockData\Currencybackup.json");
            
            using (WebClient client = new WebClient())
            {
                FileStream fs;
                CurrencyInfo curr;
                string json = client.DownloadString(url);

                curr = (new JavaScriptSerializer().Deserialize<CurrencyInfo>(json));

                if(curr.success != true)
                {
                    using (StreamReader reader = new StreamReader(backupDataLocation))
                    {
                        json = reader.ReadToEnd();
                        curr = (new JavaScriptSerializer().Deserialize<CurrencyInfo>(json));                        
                    }
                }
                
                File.WriteAllText(backupDataLocation, json);
                return curr;               
            }
        }
    }
    public class CurrencyInfo
    {
        public bool success { get; set; }
        public string terms { get; set; }
        public string privacy { get; set; }
        public int timestamp { get; set; }
        public string source { get; set; }
       
        public Dictionary<string, double> quotes { get; set; }
    }
}