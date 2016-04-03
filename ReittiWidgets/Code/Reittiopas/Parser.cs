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

        //TODO: Refactor?!
        public List<Stop> ParseStops(string input)
        {
            List<Stop> stops = new List<Stop>();

            doc.LoadXml(input);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("Station");

            foreach(XmlNode node in nodes)
            {
                Stop stop = new Stop();
                XmlAttributeCollection attributes = node.Attributes;

                foreach(XmlAttribute attribute in attributes)
                {
                    if(attribute.Name == "StationId")
                        stop.StopCode = attribute.Value;
                    if (attribute.Name == "Name")
                        stop.StopName = attribute.Value;
                }
                stops.Add(stop);
            }

            return stops; 
        }
    }
}