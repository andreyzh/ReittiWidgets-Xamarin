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
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ReittiWidgets.Code.Reittiopas
{
    /// <summary>
    /// Handles communication with Reittiopas API
    /// </summary>
    class Connector
    {
        public string Url { get; set; }

        public Task<WebResponse> GetStream()
        {
            if (Url == null)
                return null;

            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(Url);
            //EXCEPTION HAPPENS HERE!
            var response = wrGETURL.GetResponseAsync();

            //Stream objStream;
            //objStream = wrGETURL.GetResponse().GetResponseStream();

            return response;
        }
    }
}