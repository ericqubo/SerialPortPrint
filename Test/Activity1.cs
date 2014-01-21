using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using SerialPortPrint;

namespace Test
{
    [Activity(Label = "Test", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        int count = 1;
        DeviceManager dm = new DeviceManager();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

            Button btnScan = FindViewById<Button>(Resource.Id.scan);
            btnScan.Click += (o,e) => {

                dm.DoDiscovery();
            };
        }
    }
}

