using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Hardware.Usb;

using System.Collections.Generic;
using Java.Util;
using Android.Util;
using System.Text;

namespace Test.USBPrint.droid
{
    [Activity(Label = "USBPrint", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        int count = 1;
        private UsbManager myUsbManager;   //USB管理器  
        private UsbDevice myUsbDevice;  //找到的USB设备  
        private UsbInterface myInterface;
        private UsbEndpoint epOut;
        private UsbEndpoint epIn;
        UsbDeviceConnection myDeviceConnection = null;

        string TAG = "my";

        List<int> productList = new List<int>();

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            // 获取UsbManager
            myUsbManager = (UsbManager)GetSystemService(UsbService);

            enumerateDevice();

            findInterface();

            openDevice();

            assignEndpoint();

            //查找USB设备
            button.Click += (o, e) =>
            {
                int ret = -100;
                string mes = "1111111111";
                byte[] Receiveytes;

                byte[] OutBuffer;//数据
                int BufferSize;
                Encoding targetEncoding;

                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(mes); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(mes);
                byte[] cmdData = new byte[BufferSize + 5];

                //初始化打印机
                cmdData[0] = 0x1B;
                cmdData[1] = 0x40;
                //设置字符顺时旋转180度
                cmdData[2] = 0x1B;
                cmdData[3] = 0x56;
                cmdData[4] = 0;
                for (int i = 0; i < BufferSize; i++)
                {
                    cmdData[5 + i] = OutBuffer[i];
                }
                //发送数据
                sendPackage(cmdData);
            };
        }

        private void getEndpoint(UsbDeviceConnection connection, UsbInterface intf)
        {
            if (intf.GetEndpoint(1) != null)
            {
                epOut = intf.GetEndpoint(1);
            }
            if (intf.GetEndpoint(0) != null)
            {
                epIn = intf.GetEndpoint(0);
            }
        }
        /**
    * 分配端点，IN | OUT，即输入输出；此处我直接用1为OUT端点，0为IN，当然你也可以通过判断
    */
        private void assignEndpoint()
        {
            if (myInterface.GetEndpoint(1) != null)
            {
                epOut = myInterface.GetEndpoint(1);
            }
            if (myInterface.GetEndpoint(0) != null)
            {
                epIn = myInterface.GetEndpoint(0);
            }

            //Log.Debug(TAG, getString(R.string.text));
        }

        /**
         * 打开设备
         */
        private void openDevice()
        {
            if (myInterface != null)
            {
                UsbDeviceConnection conn = null;
                // 在open前判断是否有连接权限；对于连接权限可以静态分配，也可以动态分配权限，可以查阅相关资料
                if (myUsbManager.HasPermission(myUsbDevice))
                {
                    conn = myUsbManager.OpenDevice(myUsbDevice);
                }

                if (conn == null)
                {
                    return;
                }

                if (conn.ClaimInterface(myInterface, true))
                {
                    myDeviceConnection = conn; // 到此你的android设备已经连上HID设备
                    Log.Debug(TAG, "打开设备成功");
                }
                else
                {
                    conn.Close();
                }
            }
        }

        /**
         * 找设备接口
         */
        private void findInterface()
        {
            if (myUsbDevice != null)
            {
                /*
                //Log.d(TAG, "interfaceCounts : " + myUsbDevice.InterfaceCount);
                for (int i = 0; i < myUsbDevice.InterfaceCount; i++)
                {
                    
                    break;
                }
                */
                UsbInterface intf = myUsbDevice.GetInterface(1);
                myInterface = intf;
            }
        }

        /**
         * 枚举设备
         */
        private void enumerateDevice()
        {
            if (myUsbManager == null)
                return;
            IEnumerator<KeyValuePair<string, UsbDevice>> usbList = myUsbManager.DeviceList.GetEnumerator();

            // UsbDevice mUsbDevice = null;
            while (usbList.MoveNext())
            {
                KeyValuePair<string, UsbDevice> kv = usbList.Current;
                productList.Add(kv.Value.ProductId);
                if (kv.Value.ProductId == 8965)
                {
                    myUsbDevice = kv.Value;
                }
            }
        }

        private void sendPackage(byte[] command)
        {
            int ret = -100;
            int len = command.Length;

            byte[] recive=new byte[20];

            /*
            // 组织准备命令
            byte[] sendOut = Commands.OUT_S;
            sendOut[8] = (byte)(len & 0xff);
            sendOut[9] = (byte)((len >> 8) & 0xff);
            sendOut[10] = (byte)((len >> 16) & 0xff);
            sendOut[11] = (byte)((len >> 24) & 0xff);

            // 1,发送准备命令
            ret = myDeviceConnection.BulkTransfer(epOut, sendOut, 31, 10000);
            if (ret != 31)
            {
                return;
            }
            */
            // 2,发送COM
            ret = myDeviceConnection.BulkTransfer(epOut, command, len, 10000);
            if (ret != len)
            {
                return;
            }

            // 3,接收发送成功信息
            ret = myDeviceConnection.BulkTransfer(epIn, recive, 13, 10000);
            if (ret != 13)
            {
                return;
            }
        }





    }
}

