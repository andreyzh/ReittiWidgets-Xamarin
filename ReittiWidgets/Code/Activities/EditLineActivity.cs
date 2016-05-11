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
using ReittiWidgets.Code.Adapters;

namespace ReittiWidgets.Code.Activities
{
    [Activity(Label = "@string/title_activity_edit_stop",  Theme = "@style/AppTheme")]
    class EditLineActivity : Activity
    {
        internal LineListAdapter adapter;
        internal Database db;
        
        private Stop stop;
        private Switch displayStopInWidget;
        private ListView lineListView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.EditLine);

            string stopCode = Intent.GetStringExtra("stopCode");

            db = new Database();
            stop = db.GetStop(stopCode);

            displayStopInWidget = (Switch)FindViewById(Resource.Id.displayStopInWidget);
            displayStopInWidget.Click += updateStop;
            displayStopInWidget.Checked = stop.DisplayInWidget;

            // Set activity title
            Title = this.Resources.GetString(Resource.String.title_activity_edit_stop) + " " + stop.Name;

            lineListView = (ListView)FindViewById(Resource.Id.linesListView);
            lineListView.ItemClick += Clicked;
            lineListView.SetMultiChoiceModeListener(new LineListMultChoiceHandler(this));
            adapter = new LineListAdapter(this, stop.Lines);
            lineListView.Adapter = adapter;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        private void updateStop(object sender, EventArgs e)
        {
            Switch sw = (Switch)sender;

            if (stop.DisplayInWidget != sw.Checked)
            {
                stop.DisplayInWidget = sw.Checked;
                db.UpdateStop(stop);
            }
        }


        private void Clicked(object o, EventArgs e)
        {
            Toast.MakeText(this, "Hello world", ToastLength.Long);
        }
    }

    /// <summary>
    /// Handles line deletion in line list via action bar
    /// </summary>
    class LineListMultChoiceHandler : Java.Lang.Object, ListView.IMultiChoiceModeListener
    {
        EditLineActivity parent;
        Dictionary<int, Line> linesToDelete = new Dictionary<int, Line>();

        public LineListMultChoiceHandler(EditLineActivity activity)
        {
            parent = activity;
        }

        // Remove selected stops
        public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
        {
            if (linesToDelete.Count > 0)
            {
                foreach (Line line in linesToDelete.Values)
                {
                    parent.db.DeleteLine(line);
                    parent.adapter.RemoveItem(line);
                }

                parent.adapter.NotifyDataSetChanged();

                Toast.MakeText(parent, parent.Resources.GetString(Resource.String.line_deleted), ToastLength.Long).Show();

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
            mode.Title = parent.Resources.GetString(Resource.String.action_desciption_select_lines);
            return true;
        }

        public void OnDestroyActionMode(ActionMode mode)
        {
            linesToDelete.Clear();
        }

        public void OnItemCheckedStateChanged(ActionMode mode, int position, long id, bool @checked)
        {
            // Select stops to add or remove
            if (@checked)
            {
                Line line = (Line)parent.adapter.GetItem(position);
                linesToDelete.Add(position, line);
            }
            else
            {
                linesToDelete.Remove(position);
            }

            mode.Title = linesToDelete.Count.ToString() + " " + parent.Resources.GetString(Resource.String.action_desciption_lines_selected);
        }

        public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
        {
            return true;
        }
    }
}