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
        SQLiteConnection db = new SQLiteConnection("ReittiWidgets");

        public void CreateAllTables(List<object> tables)
        {
            foreach(object table in tables)
            {
                Type type = table.GetType();
                db.CreateTable<Type>();
            }
        }
    }
}