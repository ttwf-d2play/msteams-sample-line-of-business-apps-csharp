using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Airlines.XAirlines.Models
{
    public class ActionDetails
    {
        public string ActionType { get; set; }
    }
    public class AirlineActionDetails : ActionDetails
    {
        public string Id { get; set; }
    }
}