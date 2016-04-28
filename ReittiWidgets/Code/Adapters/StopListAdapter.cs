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

        public StopListAdapter(Context context, List<Stop> stopList)
        {
            layoutInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            this.context = context;
            this.stopList = stopList;
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

            /*
            if (position % 2 == 1)
                convertView.SetBackgroundColor(context.Resources.GetColor(Resource.Color.ListGreen));
            else
                convertView.SetBackgroundColor(context.Resources.GetColor(Resource.Color.ListBlue));
            */

            Stop stop = (Stop)GetItem(position);
            ((TextView)convertView.FindViewById(Resource.Id.stopNameTextView)).Text = stop.Name;

            
            foreach(Line line in stop.Lines)
            {
                // Create views and set line name
                LinearLayout stopLayout = (LinearLayout)convertView.FindViewById(Resource.Id.stopLayout);
                View lineView = layoutInflater.Inflate(Resource.Layout.list_line_layout, null, false);
                TextView lineName = (TextView)lineView.FindViewById(Resource.Id.labelLineName);
                lineName.Text = line.Number;
                stopLayout.AddView(lineView);

                // Set departure times
                TextView nextDeparture = (TextView)lineView.FindViewById(Resource.Id.labelNextDeparture);
                if (line.NextDeparture != null)
                    nextDeparture.Text = line.NextDeparture;
                else
                    nextDeparture.Text = context.GetString(Resource.String.no_time); // -

                TextView followingDeparture = (TextView)lineView.FindViewById(Resource.Id.labelFollowingDeparture);
                if (line.FollowingDeparture != null)
                    followingDeparture.Text = line.FollowingDeparture;
                else
                    followingDeparture.Text = context.GetString(Resource.String.no_time);
            }

            return convertView;
        }

        public override void NotifyDataSetChanged()
        {
            base.NotifyDataSetChanged();
        }
    }
}