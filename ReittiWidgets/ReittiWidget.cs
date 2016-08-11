using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using ReittiWidgets.Code.Services;
using System;

namespace ReittiWidgets
{
    [BroadcastReceiver(Label = "@string/WidgetName")] //, Label = "@string/widget_name"
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widget_info")]
    class ReittiWidget : AppWidgetProvider
    {
        public static readonly string ItemPosition = "item_position";
        readonly string ActionOnClick = "eu.wyndnet.ReittiWidgets.WidgetItemClick";
        readonly string UpdateAllWidgets = "update_all_widgets"; // Used for alarm based updates

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            // This code is for widget update via broadcase reciever e.g. when clicking list items
            if (intent.Action.Equals(ActionOnClick, StringComparison.OrdinalIgnoreCase))
            {
                int itemPosition = intent.GetIntExtra(ReittiWidget.ItemPosition, -1);
                if(itemPosition != -1)
                {
                    //Toast.MakeText(context, "Clicked on item " + itemPosition, ToastLength.Short).Show();

                    // We caught broadcast event, update widgets
                    ComponentName thisAppWidget = new ComponentName(context.PackageName, Class.Name);
                    AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
                    int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(thisAppWidget);

                    foreach (int appWidgetId in appWidgetIds)
                    {
                        updateAppWidget(context, appWidgetManager, appWidgetId);
                    }
                }
                
            }
        }

        // Runs on widget update
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            foreach(int widgetId in appWidgetIds)
            {
                updateAppWidget(context, appWidgetManager, widgetId);
            }
        }

        // Runs on adding first instance of widget
        public override void OnEnabled(Context context)
        {
            // Make intent with action to update all widgets
            Intent intent = new Intent(context, typeof(ReittiWidget));
            intent.SetAction(UpdateAllWidgets);
            PendingIntent pIntent = PendingIntent.GetBroadcast(context, 0, intent, 0);

            // Schedule intent every 3 minutes
            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.SetRepeating(AlarmType.Rtc, DateTime.Now.Millisecond, 180000, pIntent); //180000
        }

        // Runs when last widget is deleted
        public override void OnDisabled(Context context)
        {
            // Cancel autoupdates
            Intent intent = new Intent(context, typeof(ReittiWidget));
            intent.SetAction(UpdateAllWidgets);
            PendingIntent pIntent = PendingIntent.GetBroadcast(context, 0, intent, 0);

            AlarmManager alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            alarmManager.Cancel(pIntent);
        }

        private void updateAppWidget(Context context, AppWidgetManager appWidgetManager, int appWidgetId)
        {
            // Init view
            RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.Widget);

            SetListAdapter(remoteViews, context, appWidgetId);
            SetListClick(remoteViews, context, appWidgetId);
            SetWidgetClick(remoteViews, context, appWidgetId);

            // Notify adapter that data was changed
            appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.stopsWidgetList);

            // Instruct widget manager to update widget
            appWidgetManager.UpdateAppWidget(appWidgetId, remoteViews);
        }

        private void SetListAdapter(RemoteViews remoteViews, Context context, int appWidgetId)
        {
            // Initialize adapter provided by StopListWidgetService
            Intent svcIntent = new Intent(context, typeof(StopsListWidgetService));
            svcIntent.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);

            // Get unique signiture of intent (to differentiate should we need it)
            Android.Net.Uri uri = Android.Net.Uri.Parse(svcIntent.ToUri(IntentUriType.Scheme));
            svcIntent.SetData(uri);

            //svcIntent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);

            // Tell view to get data from adapter
            remoteViews.SetRemoteAdapter(Resource.Id.stopsWidgetList, svcIntent);
        }

        // TEST: Handle ListView clicks
        private void SetListClick(RemoteViews remoteViews, Context context, int appWidgetId)
        {
            Intent listClickIntent = new Intent(context, typeof(ReittiWidget));
            listClickIntent.SetAction(ActionOnClick);
            PendingIntent pIntent = PendingIntent.GetBroadcast(context, 0, listClickIntent, 0);
            remoteViews.SetPendingIntentTemplate(Resource.Id.stopsWidgetList, pIntent);
        }

        //Click listener for root widget area
        private void SetWidgetClick(RemoteViews remoteViews, Context context, int appWidgetId)
        {
            Intent updateIntent = new Intent(context, typeof(ReittiWidget));
            // Means we want to update widget
            updateIntent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            // Means we want to update this particular widget
            updateIntent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new int[] { appWidgetId });
            // Package into pending intent
            PendingIntent pIntent = PendingIntent.GetBroadcast(context, appWidgetId, updateIntent, 0);
            // Set control on which we want click listener to be - here ROOT layout
            remoteViews.SetOnClickPendingIntent(Resource.Id.widgetRootLayout, pIntent);
        }
    }
}