using Android.App;
using Android.Widget;
using Android.OS;

using System;
using System.Collections.Generic;
using SQLite;
using System.Data.SqlClient;
using System.Net;

namespace DatabaseTest
{
	[Activity(Label = "DatabaseTest", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			// Path to DB
			var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");
			// Makes a connection to the database
			var db = new SQLiteConnection(pathToDatabase);

			// When you choose a new drop down menu
			Spinner dropdown = FindViewById<Spinner>(Resource.Id.Roomname);
			var items = new List<string>() { "one", "two", "this is a test" };
			var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
			dropdown.Adapter = adapter;
			// roomtemp = list.roomtemp;
			// temp = list.SetTemp;
			int Temp = 70;

			TextView txtResult = FindViewById<TextView>(Resource.Id.setTemp);
			var result = createDatabase(pathToDatabase);

			ImageButton Add = FindViewById<ImageButton>(Resource.Id.Add);
			Add.Click += (object sender, EventArgs e) =>
				StartActivity(typeof(InsertInfoActivity));
			Add.Enabled = false;
			// Creates the Database
			Button CreateDB = FindViewById<Button>(Resource.Id.CreateDB);
			CreateDB.Click += (object sender, EventArgs e) =>
			{
               //WebClient client = new Webclient();
				string server = "ql9.freemysqlhosting.net";
				string port = "3306";
				string database = "sql9142393";
				string uid = "sql9142393";
				string password = "pATpALsxs2";
				string connectionString;
				connectionString = "SERVER=" + server + ";" +"PORT=" + port + ";" + "DATABASE=" +
				database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
				//SqlConnectionStringBuilder dbConString = new SqlConnectionStringBuilder();
				//dbConString.UserID = "sql9142393";
				//dbConString.Password = "pATpALsxs2";
				//dbConString.DataSource = "ql9.freemysqlhosting.net";
				//string connsqlstring = string.Format("Server=ql9.freemysqlhosting.net;Database=sql9142393;User Id=sql9142393;" +
				//									 "Password=pATpALsxs2;Integrated Security=False");
				//using (SqlConnection con = new SqlConnection(dbConString.ConnectionString))
				using (SqlConnection con = new SqlConnection(connectionString))
				{
					con.Open();

				}
				//var result = createDatabase(pathToDatabase);
				//txtResult.Text = result + "\n";
				// if the database was created ok, then enable the list and single buttons
				if (result == "Database created")
					txtResult.Text = "Database Created";
				Add.Enabled = true;
			};

			Button EditSettings = FindViewById<Button>(Resource.Id.Edit);
			EditSettings.Click += (object sender, EventArgs e) =>
				StartActivity(typeof(EditData));
			// Adds to the setTemp
			// Old Code HotButton.Click += delegate { txtResult.Text = string.Format("{0} Degrees", ++Temp); };
			ImageButton HotButton = FindViewById<ImageButton>(Resource.Id.HotButton);
			HotButton.Click += (object sender, EventArgs e) =>
			{
				txtResult.Text = string.Format("{0} Degrees", ++Temp);
				var insertdata = insertUpdateData(new Room { SetTemp = txtResult.Text }, pathToDatabase);
			};
			ImageButton ColdButton = FindViewById<ImageButton>(Resource.Id.ColdButton);
			ColdButton.Click += (object sender, EventArgs e) =>
			{
				txtResult.Text = string.Format("{0} Degrees", --Temp);
				var insertdata = insertUpdateData(new Room { SetTemp = txtResult.Text }, pathToDatabase);
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

