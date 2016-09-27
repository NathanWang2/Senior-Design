using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace App1
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        int Temperature = 1;
		int Heat = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            // Get our button from the layout resource,
            // and attach an event to it

			Button button = FindViewById<Button>(Resource.Id.MyButton);
			button.Click += delegate { button.Text = string.Format("{0} clicks!", Temperature++); };

			//EditText setTemp = FindViewById<EditText>(Resource.Id.setTemp);
			Button button1 = FindViewById<Button>(Resource.Id.button1);
			button1.Click += delegate { button1.Text = string.Format("{0} clicks!", Heat++); };

			TextView setTemp = new TextView(this);
			setTemp.Text = button1.Text;



        }
    }
}

