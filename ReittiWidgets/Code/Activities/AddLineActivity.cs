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
using ReittiWidgets.Code.Adapters;

namespace ReittiWidgets.Code.Activities
{
    [Activity(Label = "Add Line", Icon = "@drawable/ic_main", Theme = "@android:style/Theme.Material.Light.DarkActionBar")]
    class AddLineActivity : Activity
    {
        // UI Elements
        Spinner spinner;
        Spinner delaySpinner;
        Button buttonAddLine;
        CheckBox showVersionsBox;
        AutoCompleteTextView inputStopName;
        ProgressDialog progressDialog;

        // Members
        private bool dbUpdated = false;
        private bool isConnected;
        private long currentStopId;
        private String currentStopName;
        protected Dictionary<String, String> lineMap;
        protected List<Stop> allStops;
        protected String previousUrl = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddLine);

            // Load all stops from the XML asset
            allStops = getStopInformation();

            // Set default value for delay spinner
            delaySpinner = (Spinner)FindViewById(Resource.Id.spinner2);
            delaySpinner.SetSelection(2);

            showVersionsBox = (CheckBox)FindViewById(Resource.Id.checkBoxShowVersions);

            //TEST:
            var autoCompleteOptions = new String[] { "Hello", "Hey", "Heja", "Hi", "Hola", "Bonjour", "Gday", "Goodbye", "Sayonara", "Farewell", "Adios" };
            ArrayAdapter autoCompleteAdapter = new ArrayAdapter(this, Resource.Layout.autocomplete_stop_list, autoCompleteOptions);
            var autocompleteTextView = FindViewById<AutoCompleteTextView>(Resource.Id.inputStopName);
            autocompleteTextView.Adapter = autoCompleteAdapter;


            // Set adapter for autocomplete
            inputStopName = (AutoCompleteTextView)FindViewById(Resource.Id.inputStopName);
            AutoCompleteAdapter adapter = new AutoCompleteAdapter(this, Resource.Layout.autocomplete_stop_list, allStops);
            inputStopName.Adapter = adapter;

            // Set listener for autocomplete
            //stopInputTextBox.ItemClick += stopInputTextChanged;
        }

        // Reads stop names and codes from XML.
        private List<Stop> getStopInformation()
        {
            StreamReader streamReader = new StreamReader(Assets.Open("stops.xml"));
            string content = streamReader.ReadToEnd();

            Parser parser = new Parser();
            return parser.ParseStops(content);
        }

        private void stopInputTextChanged(object sender, AdapterView.ItemClickEventArgs e)
        {

        }
    }
}