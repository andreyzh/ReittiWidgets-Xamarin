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

namespace ReittiWidgets.Code.Data
{
    class Stop
    {
        private bool displayInWidget = false;
        private string id;
        private List<Line> lineList = new List<Line>();

        public bool DisplayInWidget
        {
            get
            {
                return displayInWidget;
            }

            set
            {
                displayInWidget = value;
            }
        }
        public string StopCode
        {
            get
            {
                return StopCode;
            }
            private set
            {
                StopCode = value;
            }
        }
        public string StopName
        {
            get
            {
                return StopName;
            }
            private set
            {
                StopName = value;
            }
        }
        public List<Line> Lines
        {
            get
            {
                return lineList;
            }
        }        

        public void AddLine(Line line)
        {
            lineList.Add(line);
        }
        public Line GetLine(int index)
        {
            return lineList[index];
        }
        public void SetStop(string stopCode, string stopName)
        {
            this.StopCode = stopCode;
            this.StopName = stopName;
        }
    }
}