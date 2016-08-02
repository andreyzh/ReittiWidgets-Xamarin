using Android.App;
using Android.Content;
using Android.Widget;
using ReittiWidgets.Code.Adapters;

namespace ReittiWidgets.Code.Services
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    class StopsListWidgetService : RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            return new StopListWidgetAdapter(ApplicationContext, intent);
        }
    }
}