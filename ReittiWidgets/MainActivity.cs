using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReittiWidgets.Code;
using ReittiWidgets.Code.Activities;
using ReittiWidgets.Code.Adapters;
using ReittiWidgets.Code.Data;
using ReittiWidgets.Code.Fragments;
using System.Collections.Generic;

namespace ReittiWidgets
{
    [Activity(Label = "Reitti Widgets", MainLauncher = true, Theme = "@style/AppTheme")]
    public class MainActivity : Activity
    {
        #region Views
        private ListView stopListView;
        private ProgressDialog progressDialog;
        #endregion

        #region Memebers
        internal Database db = new Database();
        internal StopListAdapter adapter;
        private readonly int REQUEST_DATABASE_UPDATE = 1;
        private readonly string TAG_TASK_FRAGMENT = "task_fragment";
        private bool isConnected;
        private DeparturesFragment departuresFragment;
        
        #endregion

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Check connectivity
            isConnected = Utils.CheckConnectivity(this);

            stopListView = FindViewById<ListView>(Resource.Id.stopsListView);
            stopListView.ItemClick += StopListView_ItemClick;
            stopListView.SetMultiChoiceModeListener(new StopListMultChoiceHandler(this));

            // Get stops from DB
            db.CreateAllTables();

            // Set fragment that stores and updates stops
            departuresFragment = (DeparturesFragment)FragmentManager.FindFragmentByTag(TAG_TASK_FRAGMENT);

            if (Utils.CheckConnectivity(this))
            {
                // Activity first start, fragment not created
                if(departuresFragment == null)
                {
                    departuresFragment = new DeparturesFragment();
                    departuresFragment.TimeTableUpdated += Timetable_Updated;
                    FragmentManager.BeginTransaction().Add(departuresFragment, TAG_TASK_FRAGMENT).Commit();
                    requestTimetableUpdate();
                }
                // Fragment exists, but has no data - case when activity has been in background
                else if(departuresFragment != null && departuresFragment.Stops == null)
                {
                    departuresFragment.TimeTableUpdated += Timetable_Updated;
                    requestTimetableUpdate();
                }
                // Fragment exists with data - case when screen is rotated
                else
                {
                    adapter = new StopListAdapter(this, departuresFragment.Stops);
                    stopListView.Adapter = adapter;
                }
            }
            else
                Toast.MakeText(this, Resources.GetString(Resource.String.no_connection), ToastLength.Long).Show();


        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnResume()
        {
            base.OnResume();

            if (adapter != null)
                adapter.NotifyDataSetChanged();
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            if(adapter != null)
                adapter.NotifyDataSetInvalidated();

            if(departuresFragment != null)
            {
                requestTimetableUpdate();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        // Inflate menu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.MainMenu, menu);
            return true;
        }

        // Menu actions
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            switch(item.ItemId)
            {
                // Add line
                case Resource.Id.action_add_line:
                    Intent intent = new Intent(this, typeof(AddLineActivity));
                    StartActivityForResult(intent, REQUEST_DATABASE_UPDATE);
                    break;
                // Refresh timetable
                case Resource.Id.action_refresh:
                    if (Utils.CheckConnectivity(this))
                    {
                        if(departuresFragment != null)
                            requestTimetableUpdate();
                        else
                        {
                            departuresFragment = new DeparturesFragment();
                            departuresFragment.TimeTableUpdated += Timetable_Updated;
                            FragmentManager.BeginTransaction().Add(departuresFragment, TAG_TASK_FRAGMENT).Commit();
                            requestTimetableUpdate();
                        }
                    }
                    else
                        Toast.MakeText(this, Resources.GetString(Resource.String.no_connection), ToastLength.Long).Show();
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        // Trigger refresh after adding line
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if(resultCode == Result.Ok)
            {
                if(requestCode == REQUEST_DATABASE_UPDATE)
                {
                    bool dbUpdated = data.GetBooleanExtra("dbUpdated", false);
                    if(dbUpdated)
                    {
                        requestTimetableUpdate(true);
                    }
                }
            }
        }

        // Inflate action menu
        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            MenuInflater.Inflate(Resource.Menu.stops_context, menu);
        }

        // Handle stop deletion
        public override bool OnContextItemSelected(IMenuItem item)
        {
            AdapterView.AdapterContextMenuInfo adapterContextMenu = (AdapterView.AdapterContextMenuInfo)item.MenuInfo;
            Stop stop = (Stop)stopListView.GetItemAtPosition(adapterContextMenu.Position);

            switch(item.ItemId)
            {
                case Resource.Id.deleteStop:
                    db.DeleteStop(stop);
                    Toast.MakeText(this, Resources.GetString(Resource.String.stop_deleted), ToastLength.Long).Show();
                    adapter.RemoveItem(adapterContextMenu.Position);
                    adapter.NotifyDataSetChanged();
                    return true;
                default:
                    return base.OnContextItemSelected(item); ;
            }
        }

        // Start edit line activity on stop click
        private void StopListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Stop stop = (Stop)stopListView.GetItemAtPosition(e.Position);
            Intent intent = new Intent(this, typeof(EditLineActivity));
            intent.PutExtra("stopCode", stop.Code); // Is this used?
            StartActivity(intent);
        }

        // Request timetable update
        private void requestTimetableUpdate(bool updateDb = false)
        {
            // Show progress dialog
            if (progressDialog == null)
                progressDialog = new ProgressDialog(this);
            progressDialog.SetMessage(Resources.GetString(Resource.String.loading));
            progressDialog.Show();

            departuresFragment.PopulateStops(updateDb);
        }

        // Process Departures Fragment event on timetable update
        private void Timetable_Updated(object sender, DepartureFragmentEventArgs e)
        {
            if (progressDialog != null)
            { 
                progressDialog.Hide();
                progressDialog.Dismiss();
            }

            if (e.NoData)
            { 
                Toast.MakeText(this, Resource.String.no_timetable_data, ToastLength.Short).Show();
                return;
            }

            // Update stops
            adapter = new StopListAdapter(this, departuresFragment.Stops);
            stopListView.Adapter = adapter;
        }
    }

    /// <summary>
    /// Handles stop deletion in stop list via action bar
    /// </summary>
    class StopListMultChoiceHandler : Java.Lang.Object, ListView.IMultiChoiceModeListener
    {
        MainActivity parent;
        Dictionary<int, Stop> stopsToDelete = new Dictionary<int, Stop>();

        public StopListMultChoiceHandler(MainActivity activity)
        {
            parent = activity;
        }

        // Remove selected stops
        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            if (stopsToDelete.Count > 0)
            {
                foreach (Stop stop in stopsToDelete.Values)
                {
                    parent.db.DeleteStop(stop);
                    parent.adapter.RemoveItem(stop);
                }

                parent.adapter.NotifyDataSetChanged();

                Toast.MakeText(parent, parent.Resources.GetString(Resource.String.stop_deleted), ToastLength.Long).Show();

                mode.Finish();

                return true;
            }
            else
                return false;
        }

        // Actions on items selected
        public bool OnCreateActionMode(ActionMode mode, IMenu menu)
        {
            parent.MenuInflater.Inflate(Resource.Menu.stops_select_menu, menu);
            mode.Title = parent.Resources.GetString(Resource.String.action_desciption_select_stops);
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            stopsToDelete.Clear();
        }

        public void OnItemCheckedStateChanged(ActionMode mode, int position, long id, bool @checked)
        {
            // Select stops to add or remove
            if(@checked)
            {
                Stop stop = (Stop)parent.adapter.GetItem(position);
                stopsToDelete.Add(position, stop);
            }
            else
            {
                stopsToDelete.Remove(position);
            }

            mode.Title = stopsToDelete.Count.ToString() + " " + parent.Resources.GetString(Resource.String.action_desciption_stops_selected);
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            return true;
        }
    }
}