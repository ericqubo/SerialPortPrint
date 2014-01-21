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

namespace SerialPortPrint
{
    /// <summary>
    /// 蓝牙设备管理类
    /// </summary>
    public class DeviceManager : Activity
    {
        /// <summary>
        /// 查找到新设备回调
        /// </summary>
        public Action<List<KeyValuePair<string, string>>> FindDeviceAdapterCallback;

        private BluetoothAdapter btAdapter;
        private DeviceReceiver receiver;

        private List<KeyValuePair<string, string>> pairedDevicesArrayAdapter;
        private List<KeyValuePair<string, string>> newDevicesArrayAdapter;


        public DeviceManager()
        {
            pairedDevicesArrayAdapter = new List<KeyValuePair<string, string>>();
            newDevicesArrayAdapter = new List<KeyValuePair<string, string>>();

            // Register for broadcasts when a device is discovered
            receiver = new DeviceReceiver(newDevicesArrayAdapter);
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
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(device.Name, device.Address);
                    pairedDevicesArrayAdapter.Add(kvp);
                }
            }
            else
            {
                //没有准备好的设备
            }
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
    }
}