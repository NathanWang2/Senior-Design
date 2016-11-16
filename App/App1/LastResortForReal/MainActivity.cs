using Android.App;
using Android.Widget;
using Android.OS;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using MySql.Data.MySqlClient;
using System.Data;
using Java.Lang;
using Android.Content;

namespace LastResortForReal
{
	[Activity(Label = "LastResortForReal", MainLauncher = true)]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);


			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			TextView setTemp = FindViewById<TextView>(Resource.Id.setTemp);
			TextView realTemp = FindViewById<TextView>(Resource.Id.realTemp);
			//var result = createDatabase(pathToDatabase);
			// Adds to the setTemp
			int Temp = 0;
			MySqlConnection connection = new MySqlConnection("Server=sql9.freemysqlhosting.net;" +
			                                                 "Port=3306;" +
			                                                 "Database=sql9144471;"+ 
															 "User Id=sql9144471;" +
			                                                 "Password=gAj17T1zmr;" +
			                                                 "charset=utf8");

			try
			{
				if (connection.State == ConnectionState.Closed)
				{
					connection.Open();
					//MySqlCommand cmd = new MySqlCommand()
					//txtResult.Text = "Database Created";
					Toast.MakeText(this, "Connected to Database", ToastLength.Short).Show();
				}
			}
			catch
			{
				Toast.MakeText(this, "Did not Connect to Database", ToastLength.Short).Show();
			}
			try
			{
				// Creates the Table
				// INSERT Schedule where the space is
				string query = "CREATE TABLE " + "homeinfo" + @"(
						RoomName VARCHAR(150) PRIMARY KEY, 
						SetTemp INT(3), 
						RoomTemp INT(3),
						
						VentStatus VARCHAR(6),
						HeatCoolOff VARCHAR(4))";
				MySqlCommand cmd = new MySqlCommand(query, connection);
				cmd.ExecuteNonQuery();
				Toast.MakeText(this, "Table Created", ToastLength.Short).Show();
			}
			catch { }

			//// Path to DB
			//var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			//var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");
			//// Makes a connection to the database
			//var db = new SQLiteConnection(pathToDatabase);

			// Pulls info from database to list them on the drop down list
			Spinner dropdown = FindViewById<Spinner>(Resource.Id.Roomname);
			var namelist = new List<string>();
			// This populates the dropdown list with all the roomnames

			try
			{
				string read = "SELECT RoomName FROM " + "homeinfo";
				MySqlCommand roomlist = new MySqlCommand(read, connection);
				MySqlDataReader rooms = roomlist.ExecuteReader();
				while (rooms.Read())
				{
					var myString = rooms.GetString(0); //The 0 stands for "the 0'th column", so the first column of the result.
					namelist.Add(myString); // Adds them to a list
				}
				var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, namelist);
				dropdown.Adapter = adapter;
				rooms.Close();

				Toast.MakeText(this, "It Worked!", ToastLength.Short).Show();
			}
			catch
			{
				namelist = new List<string>() { "Not Connected" };
				var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, namelist);
				Toast.MakeText(this, "It Broke", ToastLength.Short).Show();
			}

			// This updaates the will get the set temp for the item selected in the dropdown list
			string replaceRoomname = "";
			string currentRoomname = "";
			try
			{

				dropdown.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e)
				{
					int position = dropdown_ItemSelected(sender, e);
					currentRoomname = (string)dropdown.GetItemAtPosition(position);
					replaceRoomname = currentRoomname.Replace("'", "''");
					string DBsetTemp = "SELECT SetTemp FROM homeinfo WHERE RoomName ='" + replaceRoomname + "'";
					MySqlCommand cmdsetTemp = new MySqlCommand(DBsetTemp, connection);
					MySqlDataReader readsetTemp = cmdsetTemp.ExecuteReader();
					var realSetTemp = new List<int>();
					while (readsetTemp.Read())
					{
						var myString = readsetTemp.GetInt32(0); //The 0 stands for "the 0'th column", so the first column of the result.
						realSetTemp.Add(myString); // Adds them to a list
					}
					Temp = realSetTemp[0];
					setTemp.Text = realSetTemp[0] + " Degrees";
					readsetTemp.Close();

					DBsetTemp = "SELECT RoomTemp FROM homeinfo WHERE RoomName ='" + replaceRoomname + "'";
					cmdsetTemp = new MySqlCommand(DBsetTemp, connection);
					readsetTemp = cmdsetTemp.ExecuteReader();
					realSetTemp = new List<int>();
					while (readsetTemp.Read())
					{
						var myString = readsetTemp.GetInt32(0); //The 0 stands for "the 0'th column", so the first column of the result.
						realSetTemp.Add(myString); // Adds them to a list
					}
					Temp = realSetTemp[0];
					realTemp.Text = realSetTemp[0] + " Degrees";
				};
			}
			catch (MySqlException ex)
			{
				Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
			}
			ImageButton HotButton = FindViewById<ImageButton>(Resource.Id.HotButton);
			HotButton.Click += (object sender, EventArgs e) =>
			{
				int hotTemp = ++Temp;
				MySqlCommand hot = new MySqlCommand("UPDATE homeinfo SET SetTemp='" + hotTemp + "' WHERE RoomName = '" + replaceRoomname + "'", connection);
				hot.ExecuteNonQuery();
				setTemp.Text = string.Format("{0} Degrees", hotTemp);
				//var insertdata = insertUpdateData(new Room { SetTemp = txtResult.Text }, pathToDatabase);
			};
			ImageButton ColdButton = FindViewById<ImageButton>(Resource.Id.ColdButton);
			ColdButton.Click += (object sender, EventArgs e) =>
			{
				int coldTemp = --Temp;
				MySqlCommand cold = new MySqlCommand("UPDATE homeinfo SET SetTemp='" + coldTemp + "' WHERE RoomName = '" + replaceRoomname + "'", connection);
				cold.ExecuteNonQuery();
				setTemp.Text = string.Format("{0} Degrees", coldTemp);
				//var insertdata = insertUpdateData(new Room { SetTemp = txtResult.Text }, pathToDatabase);
			};
			// Add New Entries
			Button newEntry = FindViewById<Button>(Resource.Id.CreateDB);
			newEntry.Click += (object sender, EventArgs e) =>
			{
				StartActivity(typeof(InsertInfoActivity));
			};
			// Lets the user change the selected room information
			Button EditSettings = FindViewById<Button>(Resource.Id.Edit);
			//EditSettings.Click += (object sender, EventArgs e) =>
			//	StartActivity(typeof(EditData));
			EditSettings.Click += delegate
			{
				var Edit = new Intent(this, typeof(EditData));
				Edit.PutExtra("roomData", currentRoomname);
				//Edit.PutExtra("roomData", 15);
				StartActivity(Edit);
			};
		}
		// PUT NEW CLASSES HERE!!!
		//
		//
		//
		private int dropdown_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			//Spinner spinner = (Spinner)sender;
			//string toast = string.Format("The room is {0}", spinner.GetItemAtPosition(e.Position));
			//Toast.MakeText(this, toast, ToastLength.Long).Show();
			try
			{
				return e.Position;
			}
			catch
			{
				Toast.MakeText(this, "Position Incorrect", ToastLength.Short).Show();
				return 0;
			}
		}
	}
}

