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