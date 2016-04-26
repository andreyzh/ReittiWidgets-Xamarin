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
            //deleteLineImageButton.setOnClickListener(deleteLineClickListener);
            //showVariantsSwitch.setOnCheckedChangeListener(showVersionsListener);
            //delaySpinner.setOnItemSelectedListener(spinnerListener);

            // Set position tags for the elements
            // so that we know which view was selected
            //showVariantsSwitch.setTag(position);
            //deleteLineImageButton.setTag(position);
            //delaySpinner.setTag(position);

            //((TextView)convertView.FindViewById(Resource.Id.labelLineName)).SetText(line.Number);
            //delaySpinner.setSelection(line.getDelay() - 1);
            //showVariantsSwitch.setChecked(line.isShowVersions());

            return convertView;
        }
    }
}