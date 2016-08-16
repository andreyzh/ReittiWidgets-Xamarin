using Android.App;
using Android.OS;
using ReittiWidgets.Code.Data;
using ReittiWidgets.Code.Reittiopas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReittiWidgets.Code.Fragments
{
    /// <summary>
    /// This class serves as a wrapper for populating stops with 
    /// departure information asynchronously. 
    /// It is also used to store transient information between activity
    /// lifecycle changes.
    /// </summary>
    class DeparturesFragment : Fragment
    {
        public event EventHandler<DepartureFragmentEventArgs> TimeTableUpdated;

        private Database db = new Database();
        private List<Stop> stops;

        public List<Stop> Stops
        {
            get { return stops; }
            set { stops = value; }
        }
        public bool RequestDbUpdate { get; set; }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Retain this fragment accross configuration changes
            RetainInstance = true;
        }

        public override void OnAttach(Activity activity)
        {
            base.OnAttach(activity);
        }

        public async void PopulateStops(bool updateDb = false)
        {
            if (stops == null)
                stops = db.GetStops();
            if(updateDb)
            {
                stops.Clear();
                stops = db.GetStops();
            }

            // One task (thread) per each stop
            var tasks = new List<Task<string>>();
            // Digitransit test
            var tasksDt = new List<Task<string>>();

            // Download XML files for each stop
            foreach (Stop stop in stops)
            {
                // Reittiopas
                Connector connector = new Connector();
                connector.Url = RequestBuilder.getStopRequest(stop.Code);
                tasks.Add(connector.GetXmlStringAsync());

                // Digitransit
                string query = QueryBuilder.MakeQuery(stop.GtfsId, QueryBuilder.QueryType.Departures);
                tasksDt.Add(connector.GetGraphQlAsync(query));
            }

            // Contains set of XML files from RO
            List<string> resultXml = new List<string>(await Task.WhenAll(tasks));
            // Contains set of XML files from DT
            List<string> resultJson = new List<string>(await Task.WhenAll(tasksDt));

            DepartureFragmentEventArgs args = new DepartureFragmentEventArgs();
            if (resultXml.Count == 0)
                args.NoData = true;
            else
                args.NoData = false;

            Parser parser = new Parser();
            //stops = parser.ParseDepartureData(resultXml, stops);
            stops = parser.ParseDepartureDataJson(resultJson, stops);

            // Raise event that we're done here
            onTimeTableUpdated(args);
        }

        protected void onTimeTableUpdated(DepartureFragmentEventArgs e)
        {
            TimeTableUpdated.Invoke(this, e);
        }
    }

    /// <summary>
    /// Used to send message to activity that we cannot find anything as the operation is async
    /// </summary>
    public class DepartureFragmentEventArgs : EventArgs
    {
        public bool NoData { get; set; }
    }
}