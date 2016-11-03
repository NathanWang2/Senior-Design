using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;

using System.Data;
using System.IO;
using SQLite;
using SQLitePCL;

// How I created database https://developer.xamarin.com/guides/xamarin-forms/working-with/databases/#PCL_Android
namespace App1.Database
{
    public class Database
    {
		public class ISQLite
		{
		}

		[assembly: Dependency(typeof(SQLite_Android))]
		// ...
		public class SQLite_Android : ISQLite
		{
			public SQLite_Android() { }
			public SQLite.SQLiteConnection GetConnection()
			{
				var sqliteFilename = "TodoSQLite.db3";
				string documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // Documents folder
				var path = Path.Combine(documentsPath, sqliteFilename);
				// Create the connection
				var conn = new SQLite.SQLiteConnection(path);
				// Return the database connection
				return conn;
			}
		}
	
    }
}