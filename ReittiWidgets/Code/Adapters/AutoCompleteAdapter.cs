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
    class AutoCompleteAdapter : BaseAdapter, IFilterable
    {
        private Filter filter;
        protected List<Stop> inputStopList;
        private List<Stop> inputStopListClone;
        private LayoutInflater lInflater;

        public override int Count
        {
            get
            {
                return inputStopList.Count;
            }
        }
        public Filter Filter
        {
            get
            {
                return filter;
            }
        }

        public AutoCompleteAdapter(Context context, int textViewResourceId, List<Stop> stopList)
        {
            lInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            inputStopList = stopList;
            inputStopListClone = inputStopList;
        }

        public override Java.Lang.Object GetItem(int position)
        {
            Stop value = inputStopList[position];
            return value.Name;
        }

        public override long GetItemId(int position)
        {
            Stop stop = inputStopList[position];
            return Convert.ToInt64(stop.Code);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
                convertView = lInflater.Inflate(Resource.Layout.autocomplete_stop_list, parent, false);

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
                //Example: https://github.com/skyflyer/XamarinSimpleAdapterListFilter/blob/master/MainActivity.cs

                FilterResults filterResults = new FilterResults();

                if (!string.IsNullOrEmpty(constraint.ToString()))
                {
                    List<Stop> originalValues = new List<Stop>(parent.inputStopList);
                    filterResults.Count = originalValues.Count;
                    //filterResults.Values = originalValues;
                }
                else
                {
                    List<Stop> newValues = new List<Stop>();

                    // Note the clone - original list gets stripped
                    foreach (Stop stop in parent.inputStopListClone)
                    {
                        string lowercase = stop.Name.ToLower();
                        if (lowercase.StartsWith(constraint.ToString().ToLower()))
                            newValues.Add(stop);
                    }

                    filterResults.Count = newValues.Count;
                    //filterResults.Values = newValues;
                }
                return filterResults;
            }

            protected override void PublishResults(ICharSequence constraint, FilterResults results)
            {
                throw new NotImplementedException();
            }
        }
    }
}