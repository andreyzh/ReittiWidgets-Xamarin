using Newtonsoft.Json;
using ReittiWidgets.Code.Data;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace ReittiWidgets.Code.Reittiopas
{
    /// <summary>
    /// Note: this class works specifically with the concrete stop and
    /// line objects which is braking principle of proper programming.
    /// </summary>
    class Parser
    {
        private XmlDocument doc = new XmlDocument();

        /// <summary>
        /// Pareses stops from application internal XML file
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

        public List<Stop> ParseStopsFromJSON(string input)
        {
            List<Stop> stops = new List<Stop>();
            Data.Digitransit.Digitransit elements = JsonConvert.DeserializeObject<Data.Digitransit.Digitransit>(input);

            foreach (var element in elements.Data.stops)
            {
                Stop stop = new Stop();
                stop.Code = element.Code;
                stop.Name = element.Name;

                stops.Add(stop);
            }

            return stops;
        }

        /// <summary>
        /// Extracts all lines in a given stop
        /// </summary>
        /// <param name="input">Reittiopas XML for a stop request</param>
        /// <returns>List of line objects</returns>
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

                    line.Code = type.Trim() + number.Trim();

                    number = Regex.Replace(number, "^0+", "");

                    line.Number = number.Trim() + " " + destination;
                    
                    lines.Add(line);
                }
            }

            return lines;
        }

        /// <summary>
        /// Extracts all departures for a given stop and its lines
        /// </summary>
        /// <param name="xmlData">Reittiopas XML for a stop request</param>
        /// <param name="stops">List of stops with lines to populate departures</param>
        /// <returns>Stop with lines with list of upcoming departures</returns>
        public List<Stop> ParseDepartureData(List<string> xmlData, List<Stop> stops)
        {
            foreach(string xml in xmlData)
            {
                // Find which stop does this XML relate to
                doc.LoadXml(xml);
                XmlElement root = doc.DocumentElement;
                XmlNode stopCodeNode = root.SelectSingleNode("/response/node/code");
                Stop stop = stops.Find(s => s.Code == stopCodeNode.InnerText);

                // Stop found - work on line information
                if(stop != null)
                {
                    XmlNodeList departureNodes = root.SelectNodes("/response/node/departures/node");

                    foreach(Line line in stop.Lines)
                    {
                        List<string> departureStrings = new List<string>();

                        // Goes though all departure nodes in order
                        foreach(XmlNode node in departureNodes)
                        {
                            XmlNodeList departureDetails = node.ChildNodes;
                            XmlNode code = departureDetails.Item(0);
                            XmlNode time = departureDetails.Item(1);

                            if (code.InnerText.StartsWith(line.Code))
                                departureStrings.Add(time.InnerText);
                        }

                        // Convert recieved times and populate line timetable
                        line.SetDepartures(Utils.ConvertDepartureToDate(departureStrings));
                    }
                }
            }

            return stops;
        }

        /// <summary>
        /// Extracts all departures for a given stop and its lines
        /// </summary>
        /// <param name="jsonData">Digitransit GraphQL query result in JSON format</param>
        /// <param name="stops">List of stops with lines to populate departures</param>
        /// <returns>Stop with lines with list of upcoming departures</returns>
        public List<Stop> ParseDepartureDataJson(List<string> jsonData, List<Stop> stops)
        {
            foreach(var json in jsonData)
            {
                // Deserialize using JSON.net
                Data.Digitransit.Digitransit temp = JsonConvert.DeserializeObject<Data.Digitransit.Digitransit>(json);

                // Find stop by matching GTFS Id
                Stop stop = stops.Find(s => s.GtfsId == temp.Data.Stop.GtfsId);

                // Alternative option - loop through lines
                foreach(var line in stop.Lines)
                {
                    // Find corresponding pattern
                    var departure = temp.Data.Stop.StoptimesForPatterns.Find(dep => dep.Pattern.Code.Contains(line.Code));

                    List<DateTime> departures = new List<DateTime>();

                    // Set departure time
                    foreach (var stopTime in departure.Stoptimes)
                    {
                        DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(stopTime.ScheduledDeparture);
                        DateTime departureTime = DateTime.Today.AddHours(offset.Hour).AddMinutes(offset.Minute);
                        departures.Add(departureTime);
                    }

                    line.SetDepartures(departures);
                }

                /* Scan departures and find for matching lines
                foreach(var departure in temp.Data.Stop.StoptimesForPatterns)
                {
                    // For lines e.g. 6T there's a problem. It returns internal line 6 instead of 6T
                    Line line = stop.Lines.Find(l => departure.Pattern.Code.Contains(l.Code));

                    if(line != null)
                    { 
                        List<DateTime> departures = new List<DateTime>();

                        // Set departure time
                        foreach(var stopTime in departure.Stoptimes)
                        {
                            DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(stopTime.ScheduledDeparture);
                            DateTime departureTime = DateTime.Today.AddHours(offset.Hour).AddMinutes(offset.Minute);
                            departures.Add(departureTime);
                        }

                        line.SetDepartures(departures);
                    }
                }*/
                
            }
            
            return stops;
        }
    }
}

/* Examples
 * Buses:
 * 2150K 2:Kamppi, laituri 5
 * 2165N 2:Kamppi, laituri 54
 * 2165 2:Kamppi, laituri 54
 * 2065K 2:Espoonlahti
 * 
 * Trams:
 * 1003 1:Nordenskiöldinkatu
 * 1002 1:Nordenskiöldinkatu<
 * 1007A 1:Pasila
 */
