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

namespace ReittiWidgets.Code.Data
{
    class Line
    {
        private int delay = 5;
        private List<DateTime> departures;

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
        public string NextDeparture
        {
            get
            {
                return getNextDeparure(); //TODO
            }
        }
        public string FollowingDeparture
        {
            get
            {
                return getFollowingDeparture(); //TODO
            }
        }

        public void SetDepartures(List<DateTime> departures)
        {
            this.departures = departures;
        }
        private string getNextDeparure()
        {
            string nextDeparture = null;

            DateTime adjusted = DateTime.Now.AddMinutes(delay);

            //TODO: Refactor?
            foreach(DateTime departureTime in departures)
            {
                if(adjusted >= departureTime)
                {
                    nextDeparture = departureTime.ToString("HH:mm");
                    break;
                }
            }

            return nextDeparture;
        }
        private string getFollowingDeparture()
        {
            return null;
        }
    }
}