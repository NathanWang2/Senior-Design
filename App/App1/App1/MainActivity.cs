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
            // Get our button from the layout resource,
            // and attach an event to it
			//EditText setTemp = new EditText(this);
			var setTemp = FindViewById<EditText>(Resource.Id.setTemp);

			ImageButton coldButton = FindViewById<ImageButton>(Resource.Id.ColdButton);
			coldButton.Click += delegate { setTemp.Text = string.Format("{0} degrees", Temperature--); };

			ImageButton hotButton = FindViewById<ImageButton>(Resource.Id.HotButton);
			hotButton.Click += delegate { setTemp.Text = string.Format("{0} degrees", Temperature++); };
			//Button button1 = FindViewById<Button>(Resource.Id.button1);
			//button1.Click += delegate { setTemp.Text = string.Format("{0} degrees", Temperature--);};
        }
    }
}

