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
using System.Xml;
using ReittiWidgets.Code.Data;

namespace ReittiWidgets.Code.Reittiopas
{
    class Parser
    {
        private XmlDocument doc = new XmlDocument();

        public List<Stop> ParseStops(XmlReader inputXML)
        {
            List<Stop> stops = new List<Stop>();

            //this.doc = inputXML;

            return stops; 
        }
    }
}