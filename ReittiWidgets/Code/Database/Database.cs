using SQLite;
using System.Collections.Generic;

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

        public bool UpdateLine(Line line)
        {
            int i = db.Update(line);
            return i !=0 ? true : false;
        }

        public bool UpdateStop(Stop stop)
        {
            int i = db.Update(stop);
            return i != 0 ? true : false;
        }

        public bool DeleteLine(Line line)
        {
            int id = 0;

            id = db.Delete(line);

            return id != 0 ? true : false;
        }

        public bool DeleteStop(Stop stop)
        {
            int id = 0;
            id = db.Delete(stop);

            foreach(Line line in stop.Lines)
            {
                db.Delete(line);
            }

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

        /// <summary>
        /// Find stops with should be displayed in widget
        /// </summary>
        /// <returns>Collection of stops with lines that should be displayed in widget</returns>
        public List<Stop> GetWidgetStops()
        {
            List<Stop> widgetStops = new List<Stop>();

            // Query stops table which have display in widget enabled
            var queryResultStops = db.Table<Stop>().Where(s => s.DisplayInWidget == true);

            foreach(var stop in queryResultStops)
            {
                // Find lines for each of the stops
                var queryResultLines = db.Table<Line>().Where(line => line.StopCode == stop.Code);

                foreach (Line line in queryResultLines)
                {
                    stop.AddLine(line);
                }

                widgetStops.Add(stop);
            }

            return widgetStops;
        }

        public Stop GetStop(string stopCode)
        {
            Stop stop;
            var queryResultStop = db.Table<Stop>().Where(s => s.Code == stopCode);

            if (queryResultStop.Count() > 0)
            { 
                stop = (Stop)queryResultStop.First();

                // Get lines
                var queryResultLines = db.Table<Line>().Where(line => line.StopCode == stop.Code);

                foreach (Line line in queryResultLines)
                {
                    stop.AddLine(line);
                }

                return stop;
            }
            else
                return null;
        }
        
        private List<Line> getLinesInStop(string stopCode)
        {
            List<Line> lineList = new List<Line>();

            var queryResultLines = db.Table<Line>().Where(line => line.StopCode == stopCode);

            if(queryResultLines.Count() > 0)
                lineList.AddRange(queryResultLines);

            return lineList;
        }
    }
}