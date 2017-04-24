using Android.App;
using Android.Widget;
using Android.OS;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace WebRequestTest
{
	[Activity(Label = "WebRequestTest", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		// base server url
		private static string server_url = "https://797862787b.dataplicity.io";
		// create room url
		private static string create_room_url = $"{server_url}/create_room.php";
		// get room url
		private static string get_room_url = $"{server_url}/get_room.php";
		// Update Set Temp
		private static string update_set_temp_url = $"{server_url}/update_set_temp.php";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			TextView setTemp = FindViewById<TextView>(Resource.Id.setTemp);
			TextView realTemp = FindViewById<TextView>(Resource.Id.realTemp);
			Spinner dropdown = FindViewById<Spinner>(Resource.Id.Roomname);


			string result = string.Format("{0}", get_room("WaterBottle").data[0].setTemp) + " degrees";
			//result = string.Format(get_room("").data[0].roomName);
			//setTemp.Text = result;
			var namelist = new List<string>();

			try
			{
				for (int index = 0; index < get_room("").data.Count; index++)
				{
					namelist.Add(get_room("").data[index].roomName);
				}
				var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, namelist);
				dropdown.Adapter = adapter;

				Toast.MakeText(this, "It Worked!", ToastLength.Short).Show();
			}
			catch
			{
				namelist = new List<string>() { "Not Connected" };
				var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, namelist);
				Toast.MakeText(this, "It Broke with no names", ToastLength.Short).Show();
			}

			string currentRoomname = "";

			int Temp = 0;
			int tempPosition;
			dropdown.ItemSelected += delegate (object sender, AdapterView.ItemSelectedEventArgs e)
				{
					int position = dropdown_ItemSelected(sender, e);
					currentRoomname = (string)dropdown.GetItemAtPosition(position);
					Toast.MakeText(this, position.ToString() , ToastLength.Short).Show();
					tempPosition = get_room("").data[position].setTemp;
					Toast.MakeText(this, tempPosition.ToString(), ToastLength.Short);
					Temp = get_room("").data[position].setTemp;
					setTemp.Text = Temp.ToString();
					realTemp.Text = get_room("").data[position].roomTemp.ToString();
				//Toast.MakeText(this, get_room("").data[position].setTemp.ToString(), ToastLength.Short).Show();

				};

			//Toast.MakeText(this, get_room("").data[tempPosition].setTemp.ToString(), ToastLength.Short).Show();
			//realTemp = string.Format("{0}", namelist[1]);

			ImageButton refresh = FindViewById<ImageButton>(Resource.Id.refresh);
			refresh.Click += delegate {
				StartActivity(typeof(MainActivity));
			};


			ImageButton HotButton = FindViewById<ImageButton>(Resource.Id.HotButton);
			HotButton.Click += (object sender, EventArgs e) =>
			{
				int hotTemp = ++Temp;
				setTemp.Text = string.Format("{0} Degrees", hotTemp);
				setTempUpdate(currentRoomname, Temp);
			};
			ImageButton ColdButton = FindViewById<ImageButton>(Resource.Id.ColdButton);
			ColdButton.Click += (object sender, EventArgs e) =>
			{
				int coldTemp = --Temp;
				setTemp.Text = string.Format("{0} Degrees", coldTemp);
				setTempUpdate(currentRoomname, Temp);
			};
			// Add New Entries
			Button newEntry = FindViewById<Button>(Resource.Id.CreateDB);
			newEntry.Click += (object sender, EventArgs e) =>
			{
				StartActivity(typeof(InsertInfoActivity));
			};
			// Lets the user change the selected room information
			Button EditSettings = FindViewById<Button>(Resource.Id.Edit);
			EditSettings.Click += (object sender, EventArgs e) =>
			{
				StartActivity(typeof(EditData));
			};
			/*
			 * 		int count = 1;
			//Test for creating new room
			TextView textView1 = FindViewById<TextView>(Resource.Id.textView1);

			create_room("Hey Jordan did you change the settings?");
			
			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button>(Resource.Id.myButton);

			button.Click += delegate { button.Text = string.Format("{0} clicks!", count++);

				string result = string.Format("{0}", get_room("WaterBottle").data[0].roomName);
				textView1.Text = result;
			};
			*/
		}
		// START PUTTING CLASSES HERE

		private int dropdown_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinner = (Spinner)sender;
			string toast = string.Format("The room is {0}", spinner.GetItemAtPosition(e.Position));
			Toast.MakeText(this, toast, ToastLength.Long).Show();
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
				url += $"{parm.Key}={parm.Value.ToString()}&";
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

		static UpdateSetTemp setTempUpdate(
			string room_name,
			int set_temp)
		{
			// parameter dictionary to be appended to request url
			Dictionary<string, object> parameters = new Dictionary<string, object>()
			{
				{ "room_name", room_name },
				{ "set_temp", set_temp }
			};
			// add parms to url
			string request_url = add_parameters(update_set_temp_url, parameters);
			// make request and get json response
			string json_response = get_response(request_url);
			// deserialize json response into CreateRoomResponse object
			UpdateSetTemp response = JsonConvert.DeserializeObject<UpdateSetTemp>(json_response);
			return response;
		}

		// makes a request to get a room with the room name passed
		// static string get_room(string room_name)
		static GetRoomResponse get_room(string room_name)
		{
			// parameter dictionary to be appended to request url
			Dictionary<string, object> parameters = new Dictionary<string, object>()
			{
				{ "room_name", room_name }
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

	public class UpdateSetTemp : Response
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

