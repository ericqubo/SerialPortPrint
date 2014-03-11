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
using Android.Bluetooth;

namespace Test.BluetoothPrint.droid
{
    /// <summary>
    /// 蓝牙设备管理类
    /// </summary>
    [Activity(Label = "device",
        Theme = "@android:style/Theme.Dialog",
                ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation)]
    public class DeviceManager : Activity
    {
        // Intent request codes
        // TODO: Make into Enums
        public const int REQUEST_CONNECT_DEVICE = 1;
        public const int REQUEST_ENABLE_BT = 2;

        // Return Intent extra
        public const string EXTRA_DEVICE_ADDRESS = "device_address";
        /// <summary>
        /// 查找到新设备回调
        /// </summary>
        public Action<List<KeyValuePair<string, string>>> FindDeviceAdapterCallback;

        private BluetoothAdapter btAdapter;
        private DeviceReceiver receiver;

        private static ArrayAdapter<string> pairedDevicesArrayAdapter;
        private static ArrayAdapter<string> newDevicesArrayAdapter;

        protected override void OnCreate(Bundle bundle)
        {
          
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.BluetoothSetting);
            var scanButton = FindViewById<Button>(Resource.Id.btnScan);
            scanButton.Click += (sender, e) =>
            {
                DoDiscovery();
                //(sender as View).Visibility = ViewStates.Gone;
            };

            pairedDevicesArrayAdapter = new ArrayAdapter<string>(this,Android.Resource.Layout.SimpleExpandableListItem1);
            newDevicesArrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1);

            
            
            
            var newDevicesListView = FindViewById<ListView>(Resource.Id.listView1);
            newDevicesListView.Adapter = newDevicesArrayAdapter;
            newDevicesListView.ItemClick += newDevicesListView_ItemClick;
            
            

            var pairedDevicesListView = FindViewById<ListView>(Resource.Id.listView2);
            pairedDevicesListView.Adapter = pairedDevicesArrayAdapter;
            pairedDevicesListView.ItemClick += newDevicesListView_ItemClick;
            
            // Register for broadcasts when a device is discovered
            receiver = new DeviceReceiver();
            var filter = new IntentFilter(BluetoothDevice.ActionFound);
            RegisterReceiver(receiver, filter);

            // Register for broadcasts when discovery has finished
            filter = new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished);
            RegisterReceiver(receiver, filter);

            // Get the local Bluetooth adapter
            btAdapter = BluetoothAdapter.DefaultAdapter;

            // Get a set of currently paired devices
            var pairedDevices = btAdapter.BondedDevices;

            // If there are paired devices, add each one to the ArrayAdapter
            if (pairedDevices.Count > 0)
            {
                foreach (var device in pairedDevices)
                {
                    pairedDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                }
            }
            else
            {
                //没有准备好的设备
            }
        }

        void newDevicesListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            btAdapter.CancelDiscovery();

            // Get the device MAC address, which is the last 17 chars in the View
            var info = (e.View as TextView).Text.ToString();
            var address = info.Substring(info.Length - 17);

            // Create the result Intent and include the MAC address
            Intent intent = new Intent();
            intent.PutExtra(EXTRA_DEVICE_ADDRESS, address);

            // Set result and finish this Activity
            SetResult(Result.Ok, intent);
            Finish();
        }
       
        /// <summary>
        /// 开始搜索蓝牙适配器
        /// </summary>
        public void DoDiscovery()
        {
            if (btAdapter.IsDiscovering)
            {
                btAdapter.CancelDiscovery();
            }
            btAdapter.StartDiscovery();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Make sure we're not doing discovery anymore
            if (btAdapter != null)
            {
                btAdapter.CancelDiscovery();
            }

            // Unregister broadcast listeners
            UnregisterReceiver(receiver);
        }

        public class DeviceReceiver : BroadcastReceiver
        {
            public DeviceReceiver()
            {

            }
            public override void OnReceive(Context context, Intent intent)
            {
                string action = intent.Action;

                // When discovery finds a device
                if (action == BluetoothDevice.ActionFound)
                {
                    // Get the BluetoothDevice object from the Intent
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    // If it's already paired, skip it, because it's been listed already
                    if (device.BondState != Bond.Bonded)
                    {
                        newDevicesArrayAdapter.Add(device.Name + "\n" + device.Address);
                    }
                }
                else if (action == BluetoothAdapter.ActionDiscoveryFinished)
                {
                    if (newDevicesArrayAdapter.Count == 0)
                    {
                        newDevicesArrayAdapter.Add("没有找到设备");
                    }
                    
                   
                }
            }
        }
    }
}