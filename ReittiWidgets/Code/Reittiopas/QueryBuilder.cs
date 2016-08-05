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
    /// This class is used to form valid GraphQL queries for Digitransit API
    /// </summary>
    class QueryBuilder
    {
        public enum QueryType { Departures, Stops };

        public static string MakeQuery(string searchTerm, QueryType queryType)
        {
            string query = null;

            switch (queryType)
            {
                case QueryType.Departures:
                    query = $@"{{stop(id: ""HSL:{searchTerm}"") {{name code stoptimesForPatterns(startTime:""0000"", timeRange:0,numberOfDepartures:3) {{ pattern {{ name code }} stoptimes {{ scheduledDeparture departureDelay realtimeState realtime }} }} }} }}"; ;
                    break;
                case QueryType.Stops:
                    query = $@"{{ stops(name: ""{searchTerm}"") {{ name code patterns {{ name code directionId }} }} }}";
                    break;
                default:
                    break;
            }

            return query;
        }
    }
}