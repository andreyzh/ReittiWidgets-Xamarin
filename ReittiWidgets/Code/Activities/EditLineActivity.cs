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
using ReittiWidgets.Code.Adapters;

namespace ReittiWidgets.Code.Activities
{
    [Activity(Label = "Edit Line", Icon = "@drawable/ic_main", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
    class EditLineActivity : Activity
    {
        protected LineListAdapter adapter;

        private Database db;
        private Stop stop;

        private Switch displayStopInWidget;
        private ListView lineListView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditLine);

            string stopCode = Intent.GetStringExtra("stopCode");

            db = new Database();
            stop = db.GetStop(stopCode);

            displayStopInWidget = (Switch)FindViewById(Resource.Id.displayStopInWidget);
            //displayStopInWidget.setOnCheckedChangeListener(displayInWidgetListener);
            displayStopInWidget.Checked = stop.DisplayInWidget;

            // Set activity title
            Title = this.Resources.GetString(Resource.String.title_activity_edit_stop) + " " + stop.Name;

            lineListView = (ListView)FindViewById(Resource.Id.linesListView);
            adapter = new LineListAdapter(this, stop.Lines);
            lineListView.Adapter = adapter;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}