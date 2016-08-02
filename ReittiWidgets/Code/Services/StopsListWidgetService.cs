using Android.App;
using Android.Content;
using Android.Util;
using Android.Widget;
using ReittiWidgets.Code.Adapters;

namespace ReittiWidgets.Code.Services
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    class StopsListWidgetService : RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            Log.Debug("RW", "Entering service");
            return new StopListWidgetAdapter(ApplicationContext, intent);
        }
    }
}