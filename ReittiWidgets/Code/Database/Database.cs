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
    class Database
    {
        private SQLiteConnection db;

        public Database()
        {
            string personalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string pathToDatabase = System.IO.Path.Combine(personalFolder, "reittopasWidgets.db");
            db = new SQLiteConnection(pathToDatabase);
        }

        //TODO: Refactor
        public void CreateAllTables()
        {
            int i = db.CreateTable<Stop>();
            int j = db.CreateTable<Line>();
        }

        public bool InsertLine(Line line)
        {
            // Check if line exists
            var queryResult = db.Table<Line>().Where(qLine => qLine.Code == line.Code && qLine.StopCode == line.StopCode);

            if (queryResult.Count() > 0)
                return false;

            int id = 0;
            id = db.Insert(line);

            if (id != 0)
                return true;
            else
                return false;
        }

        public bool InsertStop(Stop stop)
        {
            // Check if line exists
            var queryResult = db.Table<Stop>().Where(qStop => qStop.Code == stop.Code);

            if (queryResult.Count() > 0)
                return false;

            int id = 0;
            id = db.Insert(stop);

            if (id != 0)
                return true;
            else
                return false;
        }

        public bool DeleteStop(Stop stop)
        {
            int id = 0;
            id = db.Delete(stop);

            return id != 0 ? true : false;
        }

        public List<Stop> GetStops()
        {
            List<Stop> allStops = new List<Stop>();

            var queryResultStops = db.Table<Stop>();

            foreach(Stop stop in queryResultStops)
            {
                var queryResultLines = db.Table<Line>().Where(line => line.StopCode == stop.Code);

                foreach(Line line in queryResultLines)
                {
                    stop.AddLine(line);
                }

                allStops.Add(stop);
            }

            return allStops;
        }
    }
}