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

namespace ReittiWidgets.Code.Reittiopas
{
    /// <summary>
    /// This class is used to form valid URLs for Reittiopas API
    /// </summary>
    class RequestBuilder
    {
        private static readonly string baseUrl = "http://api.reittiopas.fi/hsl/prod/?";
        private static readonly string login = "user=andreyzh&pass=vdm86azhk96m";
        private static readonly string userHash = "userhash=06ad5027d7a825b51685675394bd25b022521f46ecf2";
        private static readonly string format = "&format=xml";
        private static string properties = "&time_limit=360&dep_limit=20";

        /// <summary>
        /// Forms URL for accesing stop data
        /// </summary>
        /// <param name="stopId">Reittipas long stop ID</param>
        /// <returns>URL for accessing stop data in string format</returns>
        public static string getStopRequest(String stopId)
        {
            return baseUrl + login + "&request=stop" + "&code=" + stopId + properties + format;
        }
    }
}

// Request example
// "http://api.reittiopas.fi/hsl/prod/?user=andreyzh&pass=vdm86azhk96m&request=stop&code=E3106&time=1212&time_limit=360&format=xml"