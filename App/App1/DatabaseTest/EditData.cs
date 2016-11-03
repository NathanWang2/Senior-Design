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

namespace DatabaseTest
{
	[Activity(Label = "EditData")]
	public class EditData : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

			base.OnCreate(savedInstanceState);
			// Create your application here
			SetContentView(Resource.Layout.EditEntry);
			// To save a new entry in the database


		}
		// Creates the Database
		private string createDatabase(string path)
		{
			try
			{
				var connection = new SQLiteConnection(path);
				connection.CreateTable<Room>();
				return "Database created";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		// Updates single parts for database
		private string insertUpdateData(Room data, string path)
		{
			try
			{
				var db = new SQLiteConnection(path);
				if (db.Insert(data) != 0)
					db.Update(data);
				return "Single data file inserted or updated";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		// Update all components
		private string insertUpdateAllData(IEnumerable<Room> data, string path)
		{
			try
			{
				var db = new SQLiteConnection(path);
				if (db.InsertAll(data) != 0)
					db.UpdateAll(data);
				return "List of data inserted or updated";
			}
			catch (SQLiteException ex)
			{
				return ex.Message;
			}
		}

		// Find recrods from Primary key
		private int findNumberRecords(string path)
		{
			try
			{
				var db = new SQLiteConnection(path);
				// this counts all records in the database, it can be slow depending on the size of the database
				var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Person");

				// for a non-parameterless query
				// var count = db.ExecuteScalar<int>("SELECT Count(*) FROM Person WHERE FirstName="Amy");

				return count;
			}
			catch (SQLiteException)
			{
				return -1;
			}
		}
	}
}
