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
        const string ActionOnClick = "itemonclick";
        const string ItemPosition = "item_position";
        const string UpdateAllWidgets = "update_all_widgets";

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);

            // Manual widget update
            if (intent.Action.Equals(UpdateAllWidgets, StringComparison.OrdinalIgnoreCase))
            {
                // Black magic here and below
                ComponentName thisAppWidget = new ComponentName(context.PackageName, Class.Name);
                AppWidgetManager appWidgetManager = AppWidgetManager.GetInstance(context);
                int[] appWidgetIds = appWidgetManager.GetAppWidgetIds(thisAppWidget);

                foreach (int appWidgetId in appWidgetIds)
                {
                    updateAppWidget(context, appWidgetManager, appWidgetId);
                }
            }
        }

        // Runs on widget update
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);
            foreach(int i in appWidgetIds)
            {
                updateAppWidget(context, appWidgetManager, i);
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

        // Update widget
        private void updateAppWidget(Context context, AppWidgetManager appWidgetManager, int appWidgetId)
        {
            // Init view
            RemoteViews remoteViews = new RemoteViews(context.PackageName, Resource.Layout.Widget);

            // Initialize adapter created by StopListWidgetService
            Intent adapter = new Intent(context, typeof(StopsListWidgetService));
            adapter.PutExtra(AppWidgetManager.ExtraAppwidgetId, appWidgetId);
            adapter.SetAction(AppWidgetManager.ActionAppwidgetUpdate);

            // Tell view to get data from adapter
            remoteViews.SetRemoteAdapter(Resource.Id.stopsWidgetList, adapter);

            /* Click listener
            PendingIntent pIntent;
            Intent updateIntent = new Intent(context, typeof(ReittiWidget));
            updateIntent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new int[] { appWidgetId });
            updateIntent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            pIntent = PendingIntent.GetBroadcast(context, appWidgetId, updateIntent, 0);
            remoteViews.SetOnClickPendingIntent(Resource.Id.widgetRootLayout, pIntent);*/

            // Instruct widget manager to update widget
            appWidgetManager.UpdateAppWidget(appWidgetId, remoteViews);
            appWidgetManager.NotifyAppWidgetViewDataChanged(appWidgetId, Resource.Id.stopsWidgetList);
        }
    }
}