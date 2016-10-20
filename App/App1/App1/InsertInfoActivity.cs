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
using App1.ORM;

// To add information for the application
namespace App1
{
    [Activity(Label = "InsertInfoActivity")]
    public class InsertInfoActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
			SetContentView(Resource.Layout.SettingsData);
			Button SaveData = FindViewById<Button>(Resource.Id.SaveData);	
			SaveData.Click += SaveData_Click;
        }
		void SaveData_Click(object sender, EventArgs e)
		{
			EditText RoomName = FindViewById<EditText>(Resource.Id.RoomName);
			DBRepository dbr = new DBRepository();
			string result = dbr.InsertRecord(RoomName.Text);
			Toast.MakeText(this, result, ToastLength.Short).Show();
			RoomName.Text = "";

			//StartActivity(typeof(MainActivity));

		}
    }
}