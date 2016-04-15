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
using SQLite;

namespace ReittiWidgets.Code.Data
{
    class Line
    {
        private int delay = 5;
        private List<DateTime> departures;

        [Ignore]
        public bool HasDepartures
        {
            get
            {
                if (departures != null)
                    return true;
                else
                    return false;
            }
        }
        public bool ShowVersions
        {
            get; set;
        }
        public int Id { get; set; }
        public int Delay
        {
            get
            {
                return delay;
            }

            set
            {
                delay = value;
            }
        }
        [Ignore]
        public string NextDeparture
        {
            get
            {
                DateTime? result = getNextDeparture();
                return result.HasValue ? result.Value.ToString("HH : mm") : " - ";
            }
        }
        [Ignore]
        public string FollowingDeparture
        {
            get
            {
                DateTime? result = getFollowingDeparture();
                return result.HasValue ? result.Value.ToString("HH : mm") : " - ";
            }
        }
        public string Number { get; set; }
        public string Code { get; set; }
        [Indexed]
        public string StopCode { get; set; }

        public void SetDepartures(List<DateTime> departures)
        {
            this.departures = departures;
        }
        public void AddDeparture(string departureTime)
        {
        }
        private DateTime? getNextDeparture()
        {
            DateTime adjusted = DateTime.Now.AddMinutes(delay);

            //TODO: Refactor?
            foreach(DateTime departureTime in departures)
            {
                if(adjusted <= departureTime)
                {
                    return departureTime;
                }
            }

            return null;
        }
        private DateTime? getFollowingDeparture()
        {
            if(getNextDeparture().HasValue)
            { 
                DateTime nextDeparture = getNextDeparture().Value;

                foreach(DateTime departureTime in departures)
                {
                    if (departureTime > nextDeparture)
                        return departureTime;
                }
            }

            return null;
        }
    }
}