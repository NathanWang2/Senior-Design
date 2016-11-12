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
using MySql.Data.MySqlClient;

// To add information for the application
namespace DatabaseTest
{
	[Activity(Label = "InsertInfoActivity")]
	public class InsertInfoActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			string table = "homeinfo";
			MySqlConnection connection = new MySqlConnection("Server=sql9.freemysqlhosting.net;Port=3306;Database=sql9142393;" +
															   "User Id=sql9142393;Password=pATpALsxs2;charset=utf8");
			connection.Open();
			// Create your application here
			SetContentView(Resource.Layout.InsertInfo);
			var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

			Button SaveData = FindViewById<Button>(Resource.Id.SaveData);
			SaveData.Click += (object sender, EventArgs e) =>
			{
				var db = new SQLiteConnection(pathToDatabase);
				EditText roomname = FindViewById<EditText>(Resource.Id.RoomName);
				EditText roomSetTemp = FindViewById<EditText>(Resource.Id.setTemp);
				//var RoomID = insertUpdateData(new Database { RoomName = (string)RoomName.Text, 
				//	SetTemp = (string)SetTemp.Text }, pathToDatabase);
				//var insertdata = insertUpdateData(new Room { RoomName = roomname.Text, 
				//	SetTemp = roomSetTemp.Text}, pathToDatabase);

				// Writing and pushing info to database
				if (roomSetTemp.Text == "")
				{
					roomSetTemp.Text = "0";
				}
				MySqlCommand cmd = new MySqlCommand("INSERT INTO " + table +"(RoomName, SetTemp) VALUES (@RoomName, @SetTemp)",connection);
				cmd.Parameters.AddWithValue("@RoomName", roomname.Text);
				cmd.Parameters.AddWithValue("@SetTemp", roomSetTemp.Text);
				cmd.ExecuteNonQuery();
				// Retrieve Information from Table in Database var list = db.Get<Room>(Rname);
				// To retrieve certain columns from primary key list.RoomName;
				roomname.Text = "";
				roomSetTemp.Text = "";
				StartActivity(typeof(MainActivity));
			};
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