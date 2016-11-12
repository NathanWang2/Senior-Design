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

namespace DatabaseTest
{
	[Activity(Label = "EditData")]
	public class EditData : Activity
	{
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			MySqlConnection connection = new MySqlConnection("Server=sql9.freemysqlhosting.net;Port=3306;Database=sql9142393;" +
															   "User Id=sql9142393;Password=pATpALsxs2;charset=utf8");
			connection.Open();
			//string table = "homeinfo";
			//var docsFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
			//var pathToDatabase = System.IO.Path.Combine(docsFolder, "db_sqlnet.db");

			base.OnCreate(savedInstanceState);
			var roomData = Intent.Extras.GetString("roomData");
			// Create your application here
			SetContentView(Resource.Layout.EditEntry);
			// To save a new entry in the database

			TextView RoomTitle = FindViewById<TextView>(Resource.Id.RoomTitle);
			RoomTitle.Text = "Roomname that will be changed";
			TextView TitleRoomNameID = FindViewById<TextView>(Resource.Id.curentRoomname);
			TitleRoomNameID.Text = roomData;
			EditText updateInfo = FindViewById<EditText>(Resource.Id.UpdateInfo);

			Button updatebtn = FindViewById<Button>(Resource.Id.Updatebtn);
			updatebtn.Click += delegate {
			MySqlCommand update = new MySqlCommand("UPDATE homeinfo SET Roomname='" + updateInfo.Text.Replace("'","''")+
			                                       "' WHERE RoomName = '" + TitleRoomNameID.Text.Replace("'","''")+ 
			                                       "'", connection);
			update.ExecuteNonQuery();
			StartActivity(typeof(MainActivity));
			};
		}
	}
}

