using System;
using SQLite;
namespace DatabaseTest
{
	[Table("DataTable")]
	public class Room
	{
        [PrimaryKey]
        public string RoomName { get; set; }

        public string SetTemp { get; set; }

        public string RoomTemp { get; set; }

        public string Schedule { get; set; }

        public string VentStatus { get; set; }

        public string HeatCoolOff { get; set; }

		public override string ToString()
		{
			return string.Format("[Room: RoomName={0}, SetTemp={1}, RoomTemp={2}, Schedule={3}, VentStatus={4}, " +
			                     "HeatCoolOff={5}]", RoomName, SetTemp, RoomTemp, Schedule, VentStatus, HeatCoolOff);
		}

    }
}
