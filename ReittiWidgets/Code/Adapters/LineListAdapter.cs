using Android.Content;
using Android.Views;
using Android.Widget;
using ReittiWidgets.Code.Data;
using System;
using System.Collections.Generic;

namespace ReittiWidgets.Code.Adapters
{
    class LineListAdapter : BaseAdapter
    {
        private Context context;
        private Database db;
        private LayoutInflater layoutInflater;
        private List<Line> lineList;

        // UI
        private Switch showVariantsSwitch;
        private ImageButton deleteLineImageButton;
        private Spinner delaySpinner;

        public override int Count
        {
            get
            {
                return lineList.Count;
            }
        }

        public LineListAdapter(Context context, List<Line> lineList)
        {
            layoutInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            this.context = context;
            this.lineList = lineList;
            db = new Database();
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return lineList[position];
        }

        public override long GetItemId(int position)
        {
            Line line = lineList[position];
            return Convert.ToInt32(line.Code);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            convertView = layoutInflater.Inflate(Resource.Layout.editline_list_line_item, parent, false);
            Line line = lineList[position];

            // Get elements
            showVariantsSwitch = (Switch)convertView.FindViewById(Resource.Id.showVariantsSwitch);
            delaySpinner = (Spinner)convertView.FindViewById(Resource.Id.delaySpinnerEdit);
            deleteLineImageButton = (ImageButton)convertView.FindViewById(Resource.Id.imageButtonDeleteLine);

            // Add change/click listeners
            deleteLineImageButton.Click += deleteLine;
            showVariantsSwitch.CheckedChange += updateLine;
            delaySpinner.ItemSelected += updateLine;

            // Set position tags for the elements
            // so that we know which view was selected
            showVariantsSwitch.Tag = position;
            deleteLineImageButton.Tag = position;
            delaySpinner.Tag = position;

            ((TextView)convertView.FindViewById(Resource.Id.labelLineName)).Text = line.Number;
            delaySpinner.SetSelection(line.Delay - 1);
            showVariantsSwitch.Checked = line.ShowVersions;

            return convertView;
        }

        /// <summary>
        /// Removes item from view
        /// </summary>
        /// <param name="position">Position of the item in the list</param>
        public void RemoveItem(int position)
        {
            lineList.RemoveAt(position);
        }

        // Deletes selected line
        private void deleteLine(object sender, EventArgs e)
        {
            View view = (View)sender;
            int position = (int)view.Tag;

            Line line = lineList[position];
            db.DeleteLine(line);
            RemoveItem(position);
            NotifyDataSetChanged();
        }

        // Updates selected line
        private void updateLine(object sender, EventArgs e)
        {
            View view = (View)sender;
            int position = (int)view.Tag;

            Line line = lineList[position];

            var type = sender.GetType();
            
            // Select action based on sender type
            switch(type.FullName)
            {
                // Delay spinner changed
                case "Android.Widget.Spinner":
                    Spinner sp = (Spinner)sender;
                    int delay = Convert.ToInt32(sp.SelectedItem.ToString());

                    if (line.Delay != delay)
                    {
                        line.Delay = delay;
                        db.UpdateLine(line);
                    }
                    break;
                // Display all versions changed
                case "Android.Widget.Switch":
                    Switch sw = (Switch) sender;

                    // Check if there's a mismatch between settings to avoid extra DB ops
                    if (line.ShowVersions != sw.Checked)
                    {
                        line.ShowVersions = sw.Checked;
                        db.UpdateLine(line);
                    }
                    break;
            }
        }
    }
}