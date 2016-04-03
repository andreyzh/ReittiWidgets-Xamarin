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
using ReittiWidgets.Code.Data;
using ReittiWidgets.Code.Reittiopas;
using System.Xml;
using System.IO;

namespace ReittiWidgets.Code.Activities
{
    [Activity(Label = "Add Line", Icon = "@drawable/ic_main")]
    class AddLineActivity : Activity
    {
        // UI Elements
        Spinner spinner;
        Spinner delaySpinner;
        Button buttonAddLine;
        CheckBox showVersionsBox;
        AutoCompleteTextView stopInput;
        ProgressDialog progressDialog;

        // Members
        private bool dbUpdated = false;
        private bool isConnected;
        private long currentStopId;
        private String currentStopName;
        protected Dictionary<String, String> lineMap;
        protected List<Stop> stopMap;
        protected String previousUrl = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.AddLine);

            getStopInformation();
        }

        // Reads stop names and codes from XML.
        private List<Stop> getStopInformation()
        {
            StreamReader streamReader = new StreamReader(Assets.Open("stops.xml"));
            string content = streamReader.ReadToEnd();

            Parser parser = new Parser();
            return parser.ParseStops(content);
        }
    }
}