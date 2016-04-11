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
using System.Text.RegularExpressions;

namespace ReittiWidgets.Code.Reittiopas
{
    /// <summary>
    /// Note: this class works specifically with the concrete stop and
    /// line objects which is braking principle of proper programming.
    /// </summary>
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
                        stop.Code = attribute.Value;
                    if (attribute.Name == "Name")
                        stop.Name = attribute.Value;
                }
                stops.Add(stop);
            }

            return stops; 
        }

        public List<Line> ParseLinesInStop(string input)
        {
            List<Line> lines = new List<Line>();

            doc.LoadXml(input);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/response/node/lines/node");

            foreach(XmlNode node in nodes)
            {
                Line line = new Line();
                string rawOutput = node.InnerText;

                string pattern = @"(\d)(\d\d\d\D*)\d:(\w*)";
                Match match = Regex.Match(rawOutput, pattern);

                if(match.Success)
                {
                    string type = match.Groups[1].Value;
                    string number = match.Groups[2].Value;
                    string destination = match.Groups[3].Value;

                    line.Number = number.Replace("0", "").Trim() + " " + destination;
                    line.Code = type.Trim() + number.Trim();

                    lines.Add(line);
                }
            }

            return lines;
        }

        public Stop ParseLineTimesInStop(string input, Stop stop)
        {
            doc.LoadXml(input);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodes = root.SelectNodes("/response/node/departures/node");

            foreach(XmlNode node in nodes)
            {

            }

            return stop;
        }
    }
}