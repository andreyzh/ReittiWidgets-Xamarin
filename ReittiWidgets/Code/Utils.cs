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
using System.Text.RegularExpressions;

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

        public static List<DateTime> ConvertDepartureToDate(List<string> input)
        {
            List<DateTime> departureTimes = new List<DateTime>();

            foreach(string time in input)
            {
                string pattern3Chars = @"^(\d{3})$";
                string pattern4Chars = @"(\d\d)(\d\d)";

                Match match3Chars = Regex.Match(time, pattern3Chars);
                Match match4Chars = Regex.Match(time, pattern4Chars);

                // Handle early morning departures e.g. 445, 530, etc
                if (match3Chars.Success)
                {
                    string correction = "0" + match3Chars.Groups[1].Value;
                    departureTimes.Add(makeDate(correction, 0));
                }

                if (match4Chars.Success)
                {
                    switch (Convert.ToInt16(match4Chars.Groups[1].Value))
                    {
                        case 24:
                            string match = "00" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match, 1));
                            break;
                        case 25:
                            string match1 = "01" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match1, 1));
                            break;
                        case 26:
                            string match2 = "02" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match2, 1));
                            break;
                        case 27:
                            string match3 = "03" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match3, 1));
                            break;
                        case 28:
                            string match4 = "04" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match4, 1));
                            break;
                        case 29:
                            string match5 = "05" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match5, 1));
                            break;
                        case 30:
                            string match6 = "06" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match6, 1));
                            break;
                        case 31:
                            string match7 = "07" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match7, 1));
                            break;
                        case 32:
                            string match8 = "08" + match4Chars.Groups[2].Value;
                            departureTimes.Add(makeDate(match8, 1));
                            break;
                        default:
                            departureTimes.Add(makeDate(time, 0));
                            break;
                    }
                }

            }

            return departureTimes;
        }

        /// <summary>
        /// Parse string and convert to Date.
        /// Days argument is used to add one day as a way to handle non-standard Reittiopas timing.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        private static DateTime makeDate(string input, int days)
        {
            //DateTime departureTime = DateTime.Today;
            var hours = 0;
            var minutes = 0;

            string pattern = @"(\d\d)(\d\d)";

            Match match = Regex.Match(input, pattern);

            // Handle early morning departures e.g. 445, 530, etc
            if (match.Success)
            {
                hours = Convert.ToInt32(match.Groups[1].Value);
                minutes = Convert.ToInt32(match.Groups[2].Value);
            }

            DateTime departureTime = DateTime.Today.AddDays(days).AddHours(hours).AddMinutes(minutes);

            return departureTime;
        }
    }
}