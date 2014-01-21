using Android.App;
using Android.Bluetooth;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Text;

namespace SerialPortPrint
{
    /// <summary>
    /// 蓝牙数据操作
    /// </summary>
    public class BluetoothHelper : IDisposable
    {
        // Track whether Dispose has been called.
        private bool disposed = false;
        // Message types sent from the BluetoothChatService Handler
        // TODO: Make into Enums
        public const int MESSAGE_STATE_CHANGE = 1;
        public const int MESSAGE_READ = 2;
        public const int MESSAGE_WRITE = 3;
        public const int MESSAGE_DEVICE_NAME = 4;
        public const int MESSAGE_TOAST = 5;

        // Key names received from the BluetoothChatService Handler
        public const string DEVICE_NAME = "Mallcoo领号设备";
        public const string TOAST = "toast";

        // Intent request codes
        // TODO: Make into Enums
        private const int REQUEST_CONNECT_DEVICE = 1;
        private const int REQUEST_ENABLE_BT = 2;

        // Name of the connected device
        internal string connectedDeviceName = null;


        private BluetoothAdapter bluetoothAdapter = null;
        private BluetoothChatService chatService = null;
        public BluetoothHelper()
        {
            // Initialize the BluetoothChatService to perform bluetooth connections
            chatService = new BluetoothChatService(new MyHandler(this));
        }

        public bool Init(out string err)
        {
            err = "";
            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            if (bluetoothAdapter == null)
            {
                err = "蓝牙未开启";
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool Connect(string address)
        {
            BluetoothDevice device = bluetoothAdapter.GetRemoteDevice(address);
            return true;
        }
        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name='message'>
        /// A string of text to send.
        /// </param>
        public bool SendMessage(Java.Lang.String message, out string err)
        {
            err = "";
            if (message.Length() <= 0)
            {
                err = "发送的消息不能为空";
                return false;
            }
            // Check that we're actually connected before trying anything
            if (chatService.GetState() != BluetoothChatService.STATE_CONNECTED)
            {
                err = "没有连接的设备";
                return false;
            }

            // Get the message bytes and tell the BluetoothChatService to write
            byte[] send = message.GetBytes();
            chatService.Write(send);
            return true;
        }
        /// <summary>
        /// sends a image
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public bool SendImg(Bitmap bitmap, out string err)
        {
            err = "";
            if (bitmap != null)
            {
                err = "bitmap不能为null";
                return false;
            }

            if (chatService.GetState() != BluetoothChatService.STATE_CONNECTED)
            {
                err = "没有连接的设备";
                return false;
            }

            byte[] data = SerialPortPrint.Pos.POS_PrintPicture(bitmap, 384, 0);
            byte[] cmdData = new byte[data.Length + 6];
            cmdData[0] = 0x1B;
            cmdData[1] = 0x2A;
            cmdData[2] = 0x32;
            cmdData[3] = 0x20;
            cmdData[4] = 0x2;
            cmdData[5] = 0x50;
            for (int i = 0; i < data.Length; i++)
            {
                cmdData[6 + i] = data[i];
            }
            chatService.Write(data);

            return true;
        }



        // The Handler that gets information back from the BluetoothChatService
        private class MyHandler : Handler
        {
            BluetoothHelper bluetoothChat;

            public MyHandler(BluetoothHelper chat)
            {
                bluetoothChat = chat;
            }
            public override void HandleMessage(Message msg)
            {
                switch (msg.What)
                {
                    case BluetoothHelper.MESSAGE_STATE_CHANGE:
                        switch (msg.Arg1)
                        {
                            case BluetoothChatService.STATE_CONNECTED:
                                //连接成功
                                break;
                            case BluetoothChatService.STATE_CONNECTING:
                                //正在连接
                                break;
                            case BluetoothChatService.STATE_LISTEN:
                            case BluetoothChatService.STATE_NONE:
                                //连接失败
                                break;
                        }
                        break;
                    case BluetoothHelper.MESSAGE_WRITE:
                        byte[] writeBuf = (byte[])msg.Obj;
                        // construct a string from the buffer
                        var writeMessage = new Java.Lang.String(writeBuf);
                        //bluetoothChat.conversationArrayAdapter.Add("も诀: " + writeMessage);
                        break;
                    case BluetoothHelper.MESSAGE_READ:
                        byte[] readBuf = (byte[])msg.Obj;
                        // construct a string from the valid bytes in the buffer
                        var readMessage = new Java.Lang.String(readBuf, 0, msg.Arg1);

                        //bluetoothChat.conversationArrayAdapter.Add(bluetoothChat.connectedDeviceName + ":  " + readMessage);
                        break;
                    case BluetoothHelper.MESSAGE_DEVICE_NAME:
                        // save the connected device's name
                        bluetoothChat.connectedDeviceName = msg.Data.GetString(BluetoothHelper.DEVICE_NAME);
                        //Toast.MakeText(Application.Context, "Connected to " + bluetoothChat.connectedDeviceName, ToastLength.Short).Show();
                        break;
                    case BluetoothHelper.MESSAGE_TOAST:
                        //Toast.MakeText(Application.Context, msg.Data.GetString(TOAST), ToastLength.Short).Show();
                        break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.Dispose();
            }
            if (chatService != null)
                chatService.Stop();
            disposed = true;
        }
    }

}
