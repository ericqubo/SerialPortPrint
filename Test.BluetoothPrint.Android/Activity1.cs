using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Android.Bluetooth;
using Android.Graphics;
using Java.IO;
using BluetoothPrint.droid;

namespace Test.BluetoothPrint.droid
{
    [Activity(Label = "Test", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        BluetoothHelper blueH = null;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            blueH = new BluetoothHelper();
            int err =0;
            Action<string,string> ConnectedAction = new Action<string,string>((name,address) =>
            { 
                
            });
            Action<string> ConnectingAction = new Action<string>((t) =>
            { 
            
            });
            Action<string> ConnFailedAction = new Action<string>((t) =>
            { 
            
            });



            if (!blueH.Init(out err, ConnectedAction, ConnectingAction, ConnFailedAction))
            {
                //蓝牙不存在
            }
            else
            {
                if (!blueH.IsOpen())
                {
                    //打开蓝牙
                    blueH.Open(this);
                }
            }




            Button btnScan = FindViewById<Button>(Resource.Id.MyButton);
            btnScan.Click += (o, e) =>
            {
                var serverIntent = new Intent(this, typeof(DeviceManager));
                StartActivityForResult(serverIntent, DeviceManager.REQUEST_CONNECT_DEVICE);
            };
            Button Print = FindViewById<Button>(Resource.Id.Print);
            Print.Click += (o, e) =>
            {
                Java.Lang.String str = new Java.Lang.String("hello");

                blueH.SendMessage(str, out err);
               
                
                
    
                Bitmap bm = BitmapFactory.DecodeStream(Resources.Assets.Open("T.png"));
       
                blueH.SendImg(bm,out err);
            };
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case 0:
                    if (resultCode == Result.Ok)
                    {

                    }
                    break;
                case DeviceManager.REQUEST_CONNECT_DEVICE:
                    // When DeviceListActivity returns with a device to connect
                    if (resultCode == Result.Ok)
                    {
                        // Get the device MAC address
                        var address = data.Extras.GetString(DeviceManager.EXTRA_DEVICE_ADDRESS);
                        // Get the BLuetoothDevice object
                        //// Attempt to connect to the device
                        blueH.Connect(address);
                    }
                    break;
                case DeviceManager.REQUEST_ENABLE_BT:
                    // When the request to enable Bluetooth returns
                    if (resultCode == Result.Ok)
                    {
                        // Bluetooth is now enabled, so set up a chat session
                        // SetupChat();
                    }
                    else
                    {
                        // User did not enable Bluetooth or an error occured
                        //Log.Debug(TAG, "BT not enabled");
                        //Toast.MakeText(this, Resource.String.bt_not_enabled_leaving, ToastLength.Short).Show();
                        //Finish();
                    }
                    break;
            }
        }
    }
}

