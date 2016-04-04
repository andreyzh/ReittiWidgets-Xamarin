using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Object = Java.Lang.Object;

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
            matchedStops = stopList;
            allStops = matchedStops;
            filter = new StopsFilter(this);
        }

        public override Java.Lang.Object GetItem(int position)
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
                //Another one: http://xenrcode.net/2015/09/09/xamarin-adding-filtering-to-custom-adapter/
                //And another one: https://gist.github.com/Cheesebaron/9838325
                // Finally https://gist.github.com/Cheesebaron/9876783

                var filterResults = new FilterResults();

                if (constraint == null)
                {
                    var originalStops = new List<Stop>(parent.matchedStops);
                    filterResults.Count = originalStops.Count;
                    filterResults.Values = FromArray(originalStops.Select(r => r.ToJavaObject()).ToArray());
                }
                else
                {
                    List<Stop> newStops = new List<Stop>();

                    // Note the clone - original list gets stripped
                    foreach (Stop stop in parent.allStops)
                    {
                        string lowercase = stop.Name.ToLower();
                        if (lowercase.StartsWith(constraint.ToString().ToLower()))
                            newStops.Add(stop);
                    }

                    filterResults.Count = newStops.Count;
                    filterResults.Values = FromArray(newStops.Select(r => r.ToJavaObject()).ToArray());
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

/*
private ContactsTableItem[] _contactList;
private ContactsTableItem[] _originalContactList;
 
protected override FilterResults PerformFiltering(ICharSequence constraint)
{
   var returnObj = new FilterResults();
   var results = new List<ContactsTableItem>();

   if (_adapter._originalContactList == null)
      _adapter._originalContactList = _adapter._contactList;
 
   if (constraint == null) 
     return returnObj;
 
    if (_adapter._originalContactList != null && _adapter._originalContactList.Any())
    {
      // Compare constraint to all names lowercased. 
      // It they are contained they are added to results.
      results.AddRange( _adapter._originalContactList.Where(contact => contact.FullName.ToLower().Contains(constraint.ToString())));
     }
 
     returnObj.Values = FromArray(results.Select(r => r.ToJavaObject()).ToArray());
 
     returnObj.Count = results.Count;
 
     constraint.Dispose();
 
     return returnObj;
}
*/
