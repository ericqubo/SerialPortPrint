using Android.App;
using Android.Bluetooth;
using Android.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace SerialPortPrint
{
    internal class DeviceReceiver : BroadcastReceiver
    {
        private List<KeyValuePair<string, string>> newDevicesArrayAdapter;
        public DeviceReceiver(List<KeyValuePair<string, string>> newDevicesArrayAdapter)
        {
            this.newDevicesArrayAdapter = newDevicesArrayAdapter;
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
                    KeyValuePair<string, string> kvp = new KeyValuePair<string, string>(device.Name, device.Address);
                    newDevicesArrayAdapter.Add(kvp);
                }
            }
            else if (action == BluetoothAdapter.ActionDiscoveryFinished)
            {
                if (newDevicesArrayAdapter.Count == 0)
                {
                    //没有查找到设备
                }
            }
        }
    }
}
