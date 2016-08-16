using SQLite;
using System.Collections.Generic;

namespace ReittiWidgets.Code.Data
{
    class Stop : Java.Lang.Object
    {
        public readonly object Instance;
        private List<Line> lineList = new List<Line>();

        #region Properties
        public bool DisplayInWidget { get; set; }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Agency { get; set; }
        /// <summary>
        /// Reittiopas stop code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// GTFS ID of the stop used in Digitransit API
        /// Format is Agency:Code
        /// If agency is not specified "HSL" will be used"
        /// </summary>
        [Ignore]
        public string GtfsId
        {
            get
            {
                if (string.IsNullOrEmpty(Agency))
                    return "HSL:" + Code;
                else
                    return Agency + ":" + Code;
            }
        }
        public string Name { get; set; }
        [Ignore]
        public List<Line> Lines
        {
            get
            {
                return lineList;
            }
        }
        #endregion

        #region Constructors
        public Stop() { }
        public Stop(object instance)
        {
            Instance = instance;
        }
        #endregion

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