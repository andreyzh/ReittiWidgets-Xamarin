using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReittiWidgets.Code;
using ReittiWidgets.Code.Activities;
using ReittiWidgets.Code.Adapters;
using ReittiWidgets.Code.Data;
using ReittiWidgets.Code.Fragments;
using ReittiWidgets.Code.Reittiopas;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReittiWidgets
{
    [Activity(Label = "Reitti Widgets", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        // Views
        private ListView stopListView;
        private ProgressDialog progressDialog;

        // Members
        //protected FragmentManager fragmentManager;
        private readonly int REQUEST_DATABASE_UPDATE = 1;
        private readonly string TAG_TASK_FRAGMENT = "task_fragment";
        private bool isConnected;
        private List<Stop> stops;
        private Database db = new Database();
        private DeparturesFragment departuresFragment;
        private StopListAdapter adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Check connectivity
            isConnected = Utils.CheckConnectivity(this);

            stopListView = FindViewById<ListView>(Resource.Id.stopsListView);
            RegisterForContextMenu(stopListView);
            stopListView.ItemClick += StopListView_ItemClick;

            // Get stops from DB
            db.CreateAllTables();
            stops = db.GetStops();

            // Set adapter
            adapter = new StopListAdapter(this, stops);
            stopListView.Adapter = adapter;

            // Set fragment that stores and updates stops
            departuresFragment = (DeparturesFragment)FragmentManager.FindFragmentByTag(TAG_TASK_FRAGMENT);

            if (Utils.CheckConnectivity(this))
            {
                if(departuresFragment == null)
                {
                    departuresFragment = new DeparturesFragment(this);
                    FragmentManager.BeginTransaction().Add(departuresFragment, TAG_TASK_FRAGMENT).Commit();
                }
                departuresFragment.Stops = stops;
                departuresFragment.PopulateStops();
                departuresFragment.TimeTableUpdated += stopsUpdated;
                //populateStops();
            }
            else
                Toast.MakeText(this, Resources.GetString(Resource.String.no_connection), ToastLength.Long).Show();
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (adapter != null)
                adapter.NotifyDataSetChanged();
        }

        // Inflate menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return true;
        }

        // Menu actions
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            switch(item.ItemId)
            {
                // Add line
                case Resource.Id.action_add_line:
                    Intent intent = new Intent(this, typeof(AddLineActivity));
                    StartActivityForResult(intent, REQUEST_DATABASE_UPDATE);
                    break;
                // Refresh timetable
                case Resource.Id.action_refresh:
                    if (Utils.CheckConnectivity(this))
                    {
                        populateStops();
                        adapter.NotifyDataSetChanged();
                    }
                    else
                        Toast.MakeText(this, Resources.GetString(Resource.String.no_connection), ToastLength.Long).Show();
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        // Trigger refresh after adding line
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if(resultCode == Result.Ok)
            {
                if(requestCode == REQUEST_DATABASE_UPDATE && stops != null)
                {
                    bool dbUpdated = data.GetBooleanExtra("dbUpdated", false);
                    if(dbUpdated)
                    {
                        stops.Clear();
                        stops = db.GetStops();
                        populateStops();
                        adapter.NotifyDataSetChanged(); // Redundant?
                    }
                }
            }
        }

        // Inflate action menu
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            MenuInflater.Inflate(Resource.Menu.stops_context, menu);
        }

        // Handle stop deletion
        public override bool OnContextItemSelected(IMenuItem item)
        {
            AdapterView.AdapterContextMenuInfo adapterContextMenu = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            Stop stop = (Stop)stopListView.GetItemAtPosition(adapterContextMenu.Position);

            switch(item.ItemId)
            {
                case Resource.Id.deleteStop:
                    db.DeleteStop(stop);
                    Toast.MakeText(this, Resources.GetString(Resource.String.stop_deleted), ToastLength.Long).Show();
                    adapter.RemoveItem(adapterContextMenu.Position);
                    adapter.NotifyDataSetChanged();
                    return true;
                default:
                    return base.OnContextItemSelected(item); ;
            }
        }

        // Start edit line activity on stop click
        private void StopListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Stop stop = (Stop)stopListView.GetItemAtPosition(e.Position);
            Intent intent = new Intent(this, typeof(EditLineActivity));
            intent.PutExtra("stopCode", stop.Code); // Is this used?
            StartActivity(intent);
        }

        /// <summary>
        /// This is a "super function" that
        /// 1. Gets RO XML stop tables using async multi-thread task
        /// 2. Parses departure information and puts it to the stops/lines
        /// 3. Initalizes adapter to display everything
        /// </summary>
        private async void populateStops()
        {
            // Show progress dialog
            if (progressDialog == null)
                progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage("Loading, please wait");
            progressDialog.Show();

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

            if(resultXml.Count == 0)
                Toast.MakeText(this, Resource.String.no_timetable_data, ToastLength.Short).Show();

            Parser parser = new Parser();
            stops = parser.ParseDepartureData(resultXml, stops);

            progressDialog.Hide();
            progressDialog.Dismiss();

            adapter = new StopListAdapter(this, stops);
            stopListView.Adapter = adapter;
        }

        private void stopsUpdated(object sender, EventArgs e)
        {
            if (progressDialog != null)
            { 
                progressDialog.Hide();
                progressDialog.Dismiss();
            }

            stops = departuresFragment.Stops;
            adapter.NotifyDataSetChanged();
        }
    }
}

// Get our button from the layout resource,
// and attach an event to it
//Button button = FindViewById<Button>(Resource.Id.MyButton);

//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
