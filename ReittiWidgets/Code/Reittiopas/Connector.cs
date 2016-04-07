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
using System.Net.Http;

namespace ReittiWidgets.Code.Reittiopas
{
    /// <summary>
    /// Handles communication with Reittiopas API
    /// </summary>
    class Connector
    {
        public string Url { get; set; }

        public async Task<string> GetXmlStringAsync()
        {
            Task<string> response;

            if (Url == null)
                return null;
            
            using (HttpClient client = new HttpClient())
            {
                response = client.GetStringAsync(Url);
            }

            string responseContent = await response;

            return responseContent;
        }
    }
}