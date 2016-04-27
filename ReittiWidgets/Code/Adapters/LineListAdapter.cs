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
using Java.Lang;
using ReittiWidgets.Code.Data;

namespace ReittiWidgets.Code.Adapters
{
    class LineListAdapter : BaseAdapter
    {
        // Note: stopList here is actually a reference on the stopList in Routes
        private List<Line> lineList;
        private Context context;
        private LayoutInflater layoutInflater;
        private Database db;

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
            convertView = layoutInflater.Inflate(Resource.Layout.list_edit_line_layout, parent, false);
            Line line = lineList[position];

            // Get elements
            showVariantsSwitch = (Switch)convertView.FindViewById(Resource.Id.showVariantsSwitch);
            delaySpinner = (Spinner)convertView.FindViewById(Resource.Id.delaySpinnerEdit);
            deleteLineImageButton = (ImageButton)convertView.FindViewById(Resource.Id.imageButtonDeleteLine);

            // Add change/click listeners
            deleteLineImageButton.Click += deleteLine;
            //showVariantsSwitch.setOnCheckedChangeListener(showVersionsListener);
            //delaySpinner.setOnItemSelectedListener(spinnerListener);

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

        public void RemoveItem(int position)
        {
            lineList.RemoveAt(position);
        }

        private void deleteLine(object sender, EventArgs e)
        {
            View view = (View)sender;
            int position = (int)view.Tag;

            Line line = lineList[position];
            db.DeleteLine(line);
            RemoveItem(position);
            NotifyDataSetChanged();
        }
    }
}