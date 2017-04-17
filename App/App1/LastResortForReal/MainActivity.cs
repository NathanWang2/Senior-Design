using Android.App;
using Android.Widget;
using Android.OS;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

using Java.Lang;
using Android.Content;

using Newtonsoft.Json;

namespace LastResortForReal
{
	[Activity(Label = "LastResortForReal", MainLauncher = true)]
	public class MainActivity : Activity
	{

		// base server url 
        private static string server_url = "https://797862787b.dataplicity.io";
		// create room url 
		private static string create_room_url = $"{server_url}/create_room.php";
		// get room url
		private static string get_room_url = $"{server_url}/get_room.php";



		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);


			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);


			// create_room requires a room name to be passed,
            // but it can accept the other parms too. If not
            // passed, the request won't include them and the
            // defaults will be used (server side)
            create_room("Test1");

			// get_room will accept a room name to be requested
			// but if no room name is passed, the server will
			// respond with all of the rooms
			// get_room("Comfort Control Living Room");

			Console.ReadLine();

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
		// string concats parameters to url passed in
        // look at create_room and get_room for an idea of
        // how dictionary passed in looks
        static string add_parameters(string url, Dictionary<string, object> parms)
		{
			// 
			url += "?";
			foreach (KeyValuePair<string, object> parm in parms)
			{
				if (parm.Value == null) continue; // ignore null parms
				url += $"{parm.Key}=\"{parm.Value.ToString()}\"&";
			}
			return url.TrimEnd('&');
		}

		// makes a request to create a room with the passed arguments.
		// some arguments are optional and won't be added to the
		// if not passed
		static CreateRoomResponse create_room(
			string room_name,
			int? set_temp = null,
			int? room_temp = null,
			string vent_status = null,
			string heat_cool_off = null)
		{
			// parameter dictionary to be appended to request url
			Dictionary<string, object> parameters = new Dictionary<string, object>()
			{
				{ "room_name", room_name },
				{ "set_temp", set_temp },
				{ "room_temp", room_temp },
				{ "vent_status", vent_status },
				{ "heat_cool_off", heat_cool_off }
			};
			// add parms to url
			string request_url = add_parameters(create_room_url, parameters);
			// make request and get json response
			string json_response = get_response(request_url);
			// deserialize json response into CreateRoomResponse object
			CreateRoomResponse response = JsonConvert.DeserializeObject<CreateRoomResponse>(json_response);
			return response;
		}

		// makes a request to get a room with the room name passed
		static GetRoomResponse get_room(string room_name)
		{
			// parameter dictionary to be appended to request url
			Dictionary<string, object> parameters = new Dictionary<string, object>()
			{
				{ "room_name", "Test Room" }
			};
			// add parms to url
			string request_url = add_parameters(get_room_url, parameters);
			// make request and get json response
			string json_response = get_response(request_url);
			// deserialize json response into GetRoomResponse object
			GetRoomResponse response = JsonConvert.DeserializeObject<GetRoomResponse>(json_response);
			return response;
		}

		// makes the request with the given url, and get the string
		// data from the response
		// https://msdn.microsoft.com/en-us/library/system.net.webrequest(v=vs.110).aspx
		static string get_response(string url)
		{
			WebRequest request = WebRequest.Create(url);
			WebResponse response = request.GetResponse();
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string responseFromServer = reader.ReadToEnd();
			reader.Close();
			response.Close();

			return responseFromServer;
		}
	}

	// base response class. CreateRoomResponse and 
	// GetRoomResponse share a success and num_results
	// properties
	public class Response
	{
		public bool success { get; set; }
		public int num_results { get; set; }
	}

	// CreateRoomResponse's data is a list of strings
	// that contains possible errors
	public class CreateRoomResponse : Response
	{
		public List<string> data { get; set; }
	}

	// GetRoomResponse's data is a list of the rooms
	// requested
	public class GetRoomResponse : Response
	{
		public List<Room> data { get; set; }
	}

	// class to define rooms that are included in
	// GetRoomResponse's data
	public class Room
	{
		public string roomName { get; set; }
		public int setTemp { get; set; }
		public int roomTemp { get; set; }
		public string ventStatus { get; set; }
		public string heatCoolOff { get; set; }
	}
}

