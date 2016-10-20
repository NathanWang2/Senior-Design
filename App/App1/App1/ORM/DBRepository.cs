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

// How I created database https://www.youtube.com/watch?v=mAw0Fa92qNE
namespace App1.ORM
{
    public class DBRepository
    {
		// Code to create Database
		public string CreateDB()
		{
			var output = "";
			output += "Creating database if it does not exist.";
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), 
			                             "ormdatabse.db3");
			//Creating actual database
			var db = new SQLiteConnection(dbPath);
			output += "\nDatabase Created";
			return output;
		}
		// Creating the table to store data
		public string CreateTable()
		{
			try
			{
				// This part establishes a connection
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
											 "ormdatabse.db3");
				var db = new SQLiteConnection(dbPath);
				db.CreateTable<SavedTask>();
				string result = "Table Created Successfully!";
				return result;
			}
			catch (Exception ex)
			{
				return "Error" + ex.Message;
			}
		}
		// Code to insert a record
		public string InsertRecord(string task)
		{ 
			try
			{
				string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),
											 "ormdatabse.db3");
				var db = new SQLiteConnection(dbPath);

				SavedTask item = new SavedTask();
				item.Task = task;
				db.Insert(item);
				return "Data is saved";
			}
			catch (Exception ex)
			{
				return "Error, Data was not saved" + ex.Message;
			}
		}

		// Code to Get Records
		public string GetRecords()
		{
			// This part establishes a connection
			string dbPath = Path.Combine(Environment.GetFolderPath
			                             (Environment.SpecialFolder.Personal),"ormdatabse.db3");
			var db = new SQLiteConnection(dbPath);

			string output = "";
			output += "Retrieving the data with ORM";
			var table = db.Table<SavedTask>();
			foreach (var item in table)
			{
				output += "\n" + item.ID + "---" + item.Task;
			}
			return output;
		}
    }
}