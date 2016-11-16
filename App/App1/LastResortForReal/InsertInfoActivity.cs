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
	[Activity(Label = "InsertInfoActivity")]
	public class InsertInfoActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Create your application here
			string table = "homeinfo";
			MySqlConnection connection = new MySqlConnection("Server=sql9.freemysqlhosting.net;" +
															 "Port=3306;" +
															 "Database=sql9144471;" +
															 "User Id=sql9144471;" +
															 "Password=gAj17T1zmr;" +
															 "charset=utf8");
			connection.Open();
			// Create your application here
			SetContentView(Resource.Layout.InsertInfo);
			var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

			Button SaveData = FindViewById<Button>(Resource.Id.SaveData);
			SaveData.Click += (object sender, EventArgs e) =>
			{
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
				MySqlCommand cmd = new MySqlCommand("INSERT INTO " + table + "(RoomName, SetTemp) VALUES (@RoomName, @SetTemp)", connection);
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
	}
}
