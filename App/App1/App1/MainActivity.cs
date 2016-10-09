using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace App1
{
    [Activity(Label = "Comfort Control", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
		int Temperature = 70;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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
			hotButton.Click += delegate { setTemp.Text = string.Format("{0} degrees", ++Temperature); };
        }
    }
}

