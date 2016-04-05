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
using Android.Views.InputMethods;
using System.Threading.Tasks;

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
        //private bool isConnected;
        private long currentStopId;
        private String currentStopName;
        protected Dictionary<String, String> lineMap;
        protected List<Stop> allStops;
        //protected String previousUrl = null;
        protected ActivityActions activityActions;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AddLine);

            activityActions = new ActivityActions(this);

            // Load all stops from the XML asset
            allStops = getStopInformation();

            // Set default value for delay spinner
            delaySpinner = (Spinner)FindViewById(Resource.Id.spinner2);
            delaySpinner.SetSelection(2);

            showVersionsBox = (CheckBox)FindViewById(Resource.Id.checkBoxShowVersions);

            // Set adapter for autocomplete
            inputStopName = (AutoCompleteTextView)FindViewById(Resource.Id.inputStopName);
            AutoCompleteAdapter adapter = new AutoCompleteAdapter(this, Resource.Layout.autocomplete_stop_list, allStops);
            inputStopName.Adapter = adapter;

            // Set listener for autocomplete
            inputStopName.ItemClick += stopInputItemClick;
        }

        // Reads stop names and codes from XML.
        private List<Stop> getStopInformation()
        {
            StreamReader streamReader = new StreamReader(Assets.Open("stops.xml"));
            string content = streamReader.ReadToEnd();

            Parser parser = new Parser();
            return parser.ParseStops(content);
        }

        // Initiates search for lines in a selected stop TODO: REFACTOR!
        private async void searchLines()
        {
            string stopCode = currentStopId.ToString();

            if (stopCode.Length == 0)
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.enter_stop_name), ToastLength.Short).Show();
            }
            else
            {
                if (Utils.CheckConnectivity(this))
                {
                    Task<Dictionary<String, String>> task = downloadLines(stopCode);

                    // Show progress dialog
                    if (progressDialog == null)
                        progressDialog = new ProgressDialog(this);
                    progressDialog.SetMessage("Loading, please wait");
                    progressDialog.Show();

                    lineMap = await task;
                }
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.no_connection), ToastLength.Short).Show();
            }
        }

        // Click listener for autocomplete suggestions - extracts selected stop data
        private void stopInputItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            // Get selected stop details
            currentStopName = (string)e.Parent.GetItemAtPosition(e.Position);
            currentStopId = e.Parent.GetItemIdAtPosition(e.Position);

            inputStopName.Text = currentStopName;

            // Hide keyboard
            InputMethodManager imm = (InputMethodManager)GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(inputStopName.WindowToken, 0);

            // Search for lines
            Task task = new Task(searchLines);
            task.Start();
        }

        private static async void getLineInformation()
        {

        }

        private static async Task<Dictionary<String, String>> downloadLines(string stopCode)
        {
            Dictionary<String, String> lineMap = new Dictionary<string, string>();

            Connector connector = new Connector();
            connector.Url = RequestBuilder.getStopRequest(stopCode);
            var xml = await connector.GetStream();

            return lineMap;
        }
    }

    class ActivityActions
    {
        Activity activity;

        public ActivityActions(Activity activity)
        {
            this.activity = activity;
        }


    }
}