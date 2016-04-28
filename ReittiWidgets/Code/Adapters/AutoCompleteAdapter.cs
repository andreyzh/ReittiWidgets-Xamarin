using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using ReittiWidgets.Code.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = Java.Lang.Object;

namespace ReittiWidgets.Code.Adapters
{
    /// <summary>
    /// Provides autocomplete features for AutoCompleteTextView based on
    /// Stop object collected from Asset XML file
    /// </summary>
    class AutoCompleteAdapter : BaseAdapter<Stop>, IFilterable
    {
        private Filter filter;
        protected List<Stop> matchedStops;
        private List<Stop> allStops;
        private LayoutInflater lInflater;

        public override int Count
        {
            get
            {
                return matchedStops.Count;
            }
        }
        public Filter Filter
        {
            get
            {
                return filter;
            }
        }

        public override Stop this[int position]
        {
            get
            {
                return matchedStops[position];
            }
        }

        public AutoCompleteAdapter(Context context, int textViewResourceId, List<Stop> stopList)
        {
            lInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            matchedStops = allStops = stopList;
            filter = new StopsFilter(this);
        }

        public override Object GetItem(int position)
        {
            Stop value = matchedStops[position];
            return value.Name;
        }

        public override long GetItemId(int position)
        {
            Stop stop = matchedStops[position];
            return Convert.ToInt64(stop.Code);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = lInflater.Inflate(Resource.Layout.addline_list_autocomplete_item, parent, false);

            ((TextView)convertView.FindViewById(Resource.Id.textView)).Text = GetItem(position).ToString();

            return convertView;
        }

        class StopsFilter : Filter
        {
            readonly AutoCompleteAdapter parent;

            public StopsFilter(AutoCompleteAdapter parent) : base()
            {
                this.parent = parent;
            }

            protected override FilterResults PerformFiltering(ICharSequence constraint)
            {
                FilterResults filterResults = new FilterResults();
                List<Stop> filteredStops = new List<Stop>();

                if (constraint == null)
                {
                    return filterResults;
                }
                else
                {
                    // Go though each stop and check if start of the name matches with inputed char sequence
                    // If there are matches, add to match list
                    foreach (Stop stop in parent.allStops)
                    {
                        string lowercase = stop.Name.ToLower();
                        if (lowercase.StartsWith(constraint.ToString().ToLower()))
                            filteredStops.Add(stop);
                    }

                    filterResults.Count = filteredStops.Count;
                    filterResults.Values = FromArray(filteredStops.Select(r => r.ToJavaObject()).ToArray());
                }
                return filterResults;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults filterResults)
            {
                if (filterResults != null && filterResults.Count > 0)
                {
                    parent.matchedStops = filterResults.Values.ToArray<Object>().Select(r => r.ToNetObject<Stop>()).ToList();
                    parent.NotifyDataSetChanged();
                }
                else parent.NotifyDataSetInvalidated();
            }
        }
    }
}
