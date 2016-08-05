using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ReittiWidgets.Code.Data.Digitransit
{
    class Digitransit
    {
        public Data Data { get; set; }
    }

    public class Pattern
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }

    public class Stoptime
    {
        public int ScheduledDeparture { get; set; }
        public int DepartureDelay { get; set; }
        public string RealtimeState { get; set; }
        public bool Realtime { get; set; }
    }

    public class StoptimesForPattern
    {
        public Pattern Pattern { get; set; }
        public List<Stoptime> Stoptimes { get; set; }
    }

    public class Stop
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<StoptimesForPattern> StoptimesForPatterns { get; set; }
        public List<object> Patterns { get; set; }
    }

    public class Data
    {
        public Stop Stop { get; set; }
        public List<Stop> stops { get; set; }
    }
}