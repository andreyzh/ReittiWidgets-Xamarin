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
using Android.Net;

namespace ReittiWidgets.Code
{
    class Utils
    {
        /// <summary>
        /// Check if there is active internet connection available
        /// </summary>
        /// <param name="activity"></param>
        /// <returns>True if connection available, otherwise false</returns>
        public static bool CheckConnectivity(Activity activity)
        {
            ConnectivityManager connectivityManager = (ConnectivityManager)activity.GetSystemService(Context.ConnectivityService);
            NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;

            return networkInfo != null && networkInfo.IsConnectedOrConnecting;
        }
    }
}