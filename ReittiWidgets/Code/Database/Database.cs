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
    class Database
    {
        private SQLiteConnection db;

        public Database()
        {
            string personalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            string pathToDatabase = System.IO.Path.Combine(personalFolder, "reittopasWidgets.db");
            db = new SQLiteConnection(pathToDatabase);
        }

        //TODO: Refactor
        public void CreateAllTables()
        {
            int i = db.CreateTable<Stop>();
            int j = db.CreateTable<Line>();
        }
    }
}