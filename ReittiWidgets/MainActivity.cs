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

namespace ReittiWidgets
{
    [Activity(Label = "Reitti Widgets", MainLauncher = true, Icon = "@drawable/ic_main", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
    public class MainActivity : Activity
    {
        // Views
        private ListView stopListView;

        // Members
        private bool isConnected;
        private List<Stop> stops;
        private Database db = new Database();
        private readonly int REQUEST_DATABASE_UPDATE = 1;


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
    }
}

// Get our button from the layout resource,
// and attach an event to it
//Button button = FindViewById<Button>(Resource.Id.MyButton);

//button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };
