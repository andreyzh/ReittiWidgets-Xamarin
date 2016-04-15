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
using Java.Lang;
using ReittiWidgets.Code.Data;

namespace ReittiWidgets.Code.Adapters
{
    class StopListAdapter : BaseAdapter
    {
        private List<Stop> stopList;
        private Context context;
        private LayoutInflater layoutInflater;

        public override int Count
        {
            get
            {
                return stopList.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return stopList[position];
        }

        public override long GetItemId(int position)
        {
            Stop stop = stopList[position];
            return Convert.ToInt32(stop.Code);
        }

        public void RemoveItem(int position)
        {
            stopList.RemoveAt(position);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            convertView = layoutInflater.Inflate(Resource.Layout.list_stop_layout, parent, false);

            Stop stop = (Stop)GetItem(position);
            ((TextView)convertView.FindViewById(Resource.Id.stopNameTextView)).Text = stop.Name;

            return convertView;
        }
    }
}