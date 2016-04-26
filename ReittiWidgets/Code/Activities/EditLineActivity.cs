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

namespace ReittiWidgets.Code.Activities
{
    class EditLineActivity : Activity
    {
        //protected LineListAdapter adapter;

        private Database db;
        private Stop stop;

        private Switch displayStopInWidget;
        private ListView lineListView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }
    }
}