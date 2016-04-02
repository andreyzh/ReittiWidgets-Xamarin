using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using ReittiWidgets.Code.Data;

namespace ReittiWidgets
{
    [Activity(Label = "Reitti Widgets", MainLauncher = true, Icon = "@drawable/ic_main")]
    public class MainActivity : Activity
    {
        // Views
        private ListView stopListView;

        // Members
        private List<Stop> stops;
        private bool isConnected;
        private Database db = new Database();


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            stopListView = FindViewById<ListView>(Resource.Id.stopsListView);
            RegisterForContextMenu(stopListView);
            stopListView.ItemClick += StopListView_ItemClick;

            // Get stops from DB
            //List<Type> tables = new List<Type>() { Stop, Line };
            //db.CreateAllTables(tables);
            
        }

        

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return base.OnPrepareOptionsMenu(menu);
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
