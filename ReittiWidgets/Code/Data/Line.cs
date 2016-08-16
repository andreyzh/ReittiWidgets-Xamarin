using SQLite;
using System;
using System.Collections.Generic;

namespace ReittiWidgets.Code.Data
{
    class Line : Java.Lang.Object
    {
        private int delay = 5;
        private List<DateTime> departures;

        #region Properties
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
        [PrimaryKey, AutoIncrement]
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
                return result.HasValue ? result.Value.ToString("HH:mm") : " - ";
            }
        }
        [Ignore]
        public string FollowingDeparture
        {
            get
            {
                DateTime? result = getFollowingDeparture();
                return result.HasValue ? result.Value.ToString("HH:mm") : " - ";
            }
        }
        public string Number { get; set; }
        public string Headsign { get; set; }
        public string Code { get; set; }
        [Indexed]
        public string StopCode { get; set; }
        #endregion

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