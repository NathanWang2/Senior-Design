using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
// SQL Stuff
using System.Data;
using System.IO;
using App1.ORM;

// Sync with an non-local database instead of creating
namespace App1
{ 
    [Activity(Label = "Comfort Control", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
		// THIS NEEDS TO BE MADE A USER INPUT
		//int Temperature = 70;
		protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			int Temperature = 70;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
			// Get the Set Temperature Textbox
			var setTemp = FindViewById<TextView>(Resource.Id.setTemp);
			// Get our button from the layout resource,
			// and attach an event to it
			ImageButton coldButton = FindViewById<ImageButton>(Resource.Id.ColdButton);
			coldButton.Click += delegate { setTemp.Text = string.Format("{0} degrees", --Temperature); };
			// This is how to change colors for Heating and Cooling Options
			//coldButton.Click += delegate { coldButton.SetBackgroundColor(Android.Graphics.Color.WhiteSmoke); };
			ImageButton hotButton = FindViewById<ImageButton>(Resource.Id.HotButton);
			hotButton.Click += delegate { 
				// Counter for Temperature
				setTemp.Text = string.Format("{0} degrees", ++Temperature);
				//// Putting Temperature into the Shared prefrence
				//var RoomTemp = Application.Context.GetSharedPreferences("My Temp", FileCreationMode.Private);
				//var RoomTempEdit = RoomTemp.Edit();
				//string TestTemp = Convert.ToString(Temperature);
				//RoomTempEdit.PutString("Temperature", TestTemp);
				//RoomTempEdit.Commit();
			};
			// For refrence of passing data https://www.youtube.com/watch?v=rYxWSV-x65I
			// Snack bar
			//Add.Click += delegate {
			//	Android.Widget.Toast.MakeText(this, "This is how to make a pop up box", ToastLength.Short).Show();
			//};
			// Create Account to Create a new Database
			Button CreateAccount = FindViewById<Button>(Resource.Id.CreateAccount);
			CreateAccount.Click += CreateAccount_Click;  

			// Save to database
			Button Save = FindViewById<Button>(Resource.Id.Save);
			Save.Click += Save_Click;

			// Goes to InsertInfoActivity
			ImageButton Add = FindViewById<ImageButton>(Resource.Id.Add);
			Add.Click += (object sender, EventArgs e) => 
				StartActivity(typeof(InsertInfoActivity));
		}
		class DPRepository : DBRepository { }
		// Create the Database & Table
		void CreateAccount_Click(object sender, EventArgs e)
		{
			DBRepository dbr = new DPRepository();
			var result = dbr.CreateDB();
			Toast.MakeText(this, result, ToastLength.Short).Show();
			//Create's table
			DBRepository DataBase = new DBRepository();
			var CreateTable = DataBase.CreateTable();
			Toast.MakeText(this, CreateTable, ToastLength.Short).Show();
		}
		void Save_Click(object sender, EventArgs e)
		{
			DBRepository dbr = new DPRepository();
			var result = dbr.GetRecords();
			Toast.MakeText(this, result, ToastLength.Short).Show();

		}
	}
}

