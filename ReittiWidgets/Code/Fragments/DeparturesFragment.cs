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
using ReittiWidgets.Code.Data;
using System.Threading.Tasks;
using ReittiWidgets.Code.Reittiopas;

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
        public event EventHandler TimeTableUpdated;

        private Activity parentActivity;
        private List<Stop> stops;

        public List<Stop> Stops
        {
            get { return stops; }
            set { stops = value; }
        }

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

        public async void PopulateStops()
        {
            if(stops == null)
                throw  new InvalidOperationException();

            // One task (thread) per each stop
            var tasks = new List<Task<string>>();

            // Download XML files for each stop
            foreach (Stop stop in stops)
            {
                Connector connector = new Connector();
                connector.Url = RequestBuilder.getStopRequest(stop.Code);
                tasks.Add(connector.GetXmlStringAsync());
            }

            // Contains set of XML files from RO
            List<string> resultXml = new List<string>(await Task.WhenAll(tasks));

            //if (resultXml.Count == 0)
            //    Toast.MakeText(this, Resource.String.no_timetable_data, ToastLength.Short).Show();

            Parser parser = new Parser();
            stops = parser.ParseDepartureData(resultXml, stops);

            // Raise event that we're done here
            onTimeTableUpdated(EventArgs.Empty);

            //adapter = new StopListAdapter(this, stops);
            //stopListView.Adapter = adapter;
        }

        public DeparturesFragment(Activity activity)
        {
            parentActivity = activity;
        }

        protected void onTimeTableUpdated(EventArgs e)
        {
            TimeTableUpdated?.Invoke(this, e);
        }
    }
}