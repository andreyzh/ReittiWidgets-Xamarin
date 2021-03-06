using Android.Appwidget;
using Android.Content;
using Android.Views;
using Android.Widget;
using ReittiWidgets.Code.Data;
using ReittiWidgets.Code.Fragments;
using ReittiWidgets.Code.Reittiopas;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ReittiWidgets.Code.Adapters
{
    /// <summary>
    /// Provides data for the widget ListView.
    /// Stop and line basic info is taken from the database.
    /// Timetable data downloaded.
    /// </summary>
    class StopListWidgetAdapter : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        bool downloadCompleted = false;
        int widgetId;

        Context context;
        Database db;
        DeparturesFragment departuresFragment;
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
            Stop stop = stops[position];
            return Convert.ToInt32(stop.Code);
        }

        public RemoteViews GetViewAt(int position)
        {
            stopView = new RemoteViews(context.PackageName, Resource.Layout.widget_stop_list_item);

            // Click handlers
            Intent clickIntent = new Intent();
            clickIntent.PutExtra(ReittiWidget.ItemPosition, position);
            stopView.SetOnClickFillInIntent(Resource.Id.lineItemsHolderWidget, clickIntent);
            stopView.SetOnClickFillInIntent(Resource.Id.stopNameTextViewWidget, clickIntent);

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

                if (line.HasDepartures)
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
            stops = new List<Stop>();
        }

        public void OnDataSetChanged()
        {
            stops.Clear();
            stops = db.GetWidgetStops();
            PopulateDepartures();
        }

        public void OnDestroy()
        {
        }

        private void PopulateDepartures()
        {
            //if (stops == null)
            //    stops = db.GetStops();

            List<string> resultXml = new List<string>();

            foreach (Stop stop in stops)
            {
                Connector connector = new Connector();
                connector.Url = RequestBuilder.getStopRequest(stop.Code);
                resultXml.Add(connector.GetXmlStringSync());
            }

            Parser parser = new Parser();
            stops = parser.ParseDepartureData(resultXml, stops);
        }
    }
}