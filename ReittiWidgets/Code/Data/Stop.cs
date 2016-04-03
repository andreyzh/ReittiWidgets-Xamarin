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
using SQLite;

namespace ReittiWidgets.Code.Data
{
    class Stop
    {
        private List<Line> lineList = new List<Line>();

        public bool DisplayInWidget { get; set; }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        [Ignore]
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
    }
}