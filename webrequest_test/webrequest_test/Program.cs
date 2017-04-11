using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace webrequest_test
{
    class Program
    {
        // base server url
        private static string server_url = "https://797862787b.dataplicity.io";
        // create room url
        private static string create_room_url = $"{server_url}/create_room.php";
        // get room url
        private static string get_room_url = $"{server_url}/get_room.php";

        static void Main(string[] args)
        {
            // create_room requires a room name to be passed,
            // but it can accept the other parms too. If not
            // passed, the request won't include them and the
            // defaults will be used (server side)
            create_room("Comfort Control Living Room"); 

            // get_room will accept a room name to be requested
            // but if no room name is passed, the server will
            // respond with all of the rooms
            // get_room("Comfort Control Living Room");

            Console.ReadLine();
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
    public class CreateRoomResponse: Response
    {
        public List<string> data { get; set; }
    }

    // GetRoomResponse's data is a list of the rooms
    // requested
    public class GetRoomResponse: Response
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