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
using Android.Appwidget;

namespace ReittiWidgets.Code.Adapters
{
    class StopListWidgetAdapter : RemoteViewsService.IRemoteViewsFactory
    {
        bool downloadCompleted = false;
        int widgetId;

        Context context;
        Database db;
        LayoutInflater layoutInflater;
        RemoteViews stopView;

        List<Stop> stops;

        #region Properties
        public int Count
        {
            get
            {
                return stops.Count;
            }
        }

        public bool HasStableIds
        {
            get
            {
                return false;
            }
        }

        public RemoteViews LoadingView
        {
            get
            {
                return null;
            }
        }

        public int ViewTypeCount
        {
            get
            {
                return 1;
            }
        }

        public IntPtr Handle
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        // Constructor
        public StopListWidgetAdapter(Context context, Intent intent)
        {
            this.context = context;
            layoutInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            widgetId = intent.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId);
        }

        public long GetItemId(int position)
        {
            throw new NotImplementedException();
        }

        public RemoteViews GetViewAt(int position)
        {
            stopView = new RemoteViews(context.PackageName, Resource.Layout.Widget);

            // Set stop name
            Stop stop = stops[position];
            stopView.SetTextViewText(Resource.Id.stopNameTextViewWidget, stop.Name);
            // Reset line views
            stopView.RemoveAllViews(Resource.Id.lineItemsHolderWidget);

            // Set line name and departure times
            foreach(var line in stop.Lines)
            {
                RemoteViews lineView = new RemoteViews(context.PackageName, Resource.Layout.widget_line_list_item);
                lineView.SetTextViewText(Resource.Id.labelLineNameWidget, line.Number);

                if(line.HasDepartures)
                {
                    if (line.NextDeparture != null)
                        lineView.SetTextViewText(Resource.Id.labelNextDepartureWidget, line.NextDeparture);
                    else
                        lineView.SetTextViewText(Resource.Id.labelNextDepartureWidget, context.GetString(Resource.String.no_time));

                    if (line.FollowingDeparture != null)
                        lineView.SetTextViewText(Resource.Id.labelFollowingDepartureWidget, line.FollowingDeparture);
                    else
                        lineView.SetTextViewText(Resource.Id.labelFollowingDepartureWidget, context.GetString(Resource.String.no_time));
                }
                else
                {
                    lineView.SetTextViewText(Resource.Id.labelNextDepartureWidget, context.GetString(Resource.String.no_time));
                    lineView.SetTextViewText(Resource.Id.labelFollowingDepartureWidget, context.GetString(Resource.String.no_time));
                }

                stopView.AddView(Resource.Id.lineItemsHolderWidget, lineView);
            }

            return stopView;
        }

        public void OnCreate()
        {
            db = new Database();
            stops = db.GetWidgetStops();
        }

        public void OnDataSetChanged()
        {
            throw new NotImplementedException();
        }

        public void OnDestroy()
        {
        }

        public void Dispose()
        {
        }
    }
}