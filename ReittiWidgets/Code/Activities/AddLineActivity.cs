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
using System.Net;

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
        private long currentStopId;
        private String currentStopName;
        protected List<Line> linesInStop;
        protected List<Stop> allStops;
        private string temp;

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

            // Set adapter for autocomplete
            inputStopName = (AutoCompleteTextView)FindViewById(Resource.Id.inputStopName);
            AutoCompleteAdapter adapter = new AutoCompleteAdapter(this, Resource.Layout.autocomplete_stop_list, allStops);
            inputStopName.Adapter = adapter;

            // Set listener for autocomplete
            inputStopName.ItemClick += stopInputItemClick;

            buttonAddLine = (Button)FindViewById(Resource.Id.buttonAddline);
            buttonAddLine.Click += addLine;
        }

        private void addLine(object sender, EventArgs e)
        {
            if(spinner != null)
            {
                Database database = new Database();
                Stop stop = allStops.Find(s => s.Code == currentStopId.ToString());

                string lineName = (string)spinner.SelectedItem;
                Line line = linesInStop.Find(l => l.Number == lineName);
                line.ShowVersions = showVersionsBox.Checked;
                line.Delay = Convert.ToInt32(delaySpinner.SelectedItem.ToString());

                bool stopInserted = database.InsertStop(stop);
                bool lineInserted = database.InsertLine(line);

                if(stopInserted || lineInserted)
                {
                    dbUpdated = true;
                    Toast.MakeText(this, "Line succesfully added", ToastLength.Short).Show();

                    Intent.PutExtra("dbUpdated", dbUpdated);
                    SetResult(Result.Ok, Intent);
                }
                if(!stopInserted && !lineInserted)
                    Toast.MakeText(this, "Line already exists", ToastLength.Short).Show();
            }
        }

        // Get lines for the selected stop and show on spinner
        private async void getLines()
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
                    // Show progress dialog
                    if (progressDialog == null)
                        progressDialog = new ProgressDialog(this);
                    progressDialog.SetMessage("Loading, please wait");
                    progressDialog.Show();

                    try
                    {
                        // Get stop information from reittiopas
                        Connector connector = new Connector();
                        connector.Url = RequestBuilder.getStopRequest(stopCode);
                        string resultXml = await connector.GetXmlStringAsync();

                        progressDialog.Hide();
                        progressDialog.Dismiss();

                        // Parse lines
                        Parser parser = new Parser();
                        this.linesInStop = parser.ParseLinesInStop(resultXml);

                        // Display on spinner
                        spinner = (Spinner)FindViewById(Resource.Id.spinner);
                        List<String> lines = linesInStop.Select(line => line.Number).ToList(); // LAMBDA!
                        ArrayAdapter<string> spinnerAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, lines);
                        spinner.Adapter = spinnerAdapter;
                    }
                    catch (Exception ex)
                    {
                        progressDialog.Hide();
                        progressDialog.Dismiss();
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }
                }
                else
                    Toast.MakeText(this, Resources.GetString(Resource.String.no_connection), ToastLength.Short).Show();
            }
        }

        // Reads stop names and codes from XML.
        private List<Stop> getStopInformation()
        {
            StreamReader streamReader = new StreamReader(Assets.Open("stops.xml"));
            string content = streamReader.ReadToEnd();

            Parser parser = new Parser();
            return parser.ParseStops(content);
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
            getLines();
        }
    }
}