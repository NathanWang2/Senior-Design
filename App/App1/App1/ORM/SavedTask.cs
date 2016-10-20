using System;
using System.Data;
using System.IO;
using SQLite;

namespace App1
{
	// This will set the table for the saved items
	[Table("SavedRooms")]
	public class SavedTask
	{
		[PrimaryKey,AutoIncrement,Column("_ID")]
		public int ID { get; set;}

		[MaxLength(50)]
		public string Task { get; set;}
	}
}
