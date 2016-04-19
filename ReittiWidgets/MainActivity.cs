using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using ReittiWidgets.Code.Data;
using ReittiWidgets.Code;
using ReittiWidgets.Code.Activities;
using ReittiWidgets.Code.Fragments;
using ReittiWidgets.Code.Reittiopas;
using System.Threading.Tasks;
using ReittiWidgets.Code.Adapters;

namespace ReittiWidgets
{
    [Activity(Label = "Reitti Widgets", MainLauncher = true, Icon = "@drawable/ic_main", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
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

            // Fragment magic
            departuresFragment = (DeparturesFragment)FragmentManager.FindFragmentByTag(TAG_TASK_FRAGMENT);

            if (Utils.CheckConnectivity(this))
            {
                populateStops();
            }
            else
                Toast.MakeText(this, Resources.GetString(Resource.String.no_connection), ToastLength.Long).Show();
        }

        // Inflate menu
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return base.OnPrepareOptionsMenu(menu);
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

            if(item.ItemId == Resource.Id.action_add_line)
            {
                Intent intent = new Intent(this, typeof(AddLineActivity));
                StartActivityForResult(intent, REQUEST_DATABASE_UPDATE);
                //StartActivity(typeof(AddLineActivity));
            }
            return base.OnOptionsItemSelected(item);
        }

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

        //TODO: deal with this magic
        private void StopListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        // Fetch and parse XML with stop information async. Put into adapter
        //FIXME: Too much functionality in one place
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
    }
}

// Get our button from the layout resource,
// and attach an event to it
//Button button = FindViewById<Button>(Resource.Id.MyButton);

//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
