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

            if(item.ItemId == Resource.Id.action_add_line)
            {
                Intent intent = new Intent(this, typeof(AddLineActivity));
                StartActivityForResult(intent, REQUEST_DATABASE_UPDATE);
                //StartActivity(typeof(AddLineActivity));
            }
            return base.OnOptionsItemSelected(item);
        }

        //TODO: deal with this magic
        private void StopListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async void populateStops()
        {
            // Show progress dialog
            if (progressDialog == null)
                progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage("Loading, please wait");
            progressDialog.Show();

            // Iterate though stops
            foreach (Stop stop in stops)
            {
                Connector connector = new Connector();
                connector.Url = RequestBuilder.getStopRequest(stop.Code);
                string resultXml = await connector.GetXmlStringAsync();
                
                foreach(Line line in stop.Lines)
                {

                }
            }

            progressDialog.Hide();
            progressDialog.Dismiss();
            // Make requests for each stop
            // Find matching lines
        }
    }
}

// Get our button from the layout resource,
// and attach an event to it
//Button button = FindViewById<Button>(Resource.Id.MyButton);

//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
