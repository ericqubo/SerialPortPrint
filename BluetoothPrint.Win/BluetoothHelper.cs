using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using PrintBase;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothPrint
{
    public class BluetoothHelper:IDisposable
    {
        private bool disposed = false;
        /// <summary>
        /// 搜索结束回调
        /// </summary>
        private BluetoothClient blueClient;
        Dictionary<string, BluetoothAddress> deviceAddress = new Dictionary<string, BluetoothAddress>();

        public bool Inite()
        {
            blueClient = new BluetoothClient();
            return true;
        }


        /// <summary>
        /// 异步扫描蓝牙设备
        /// </summary>
        public void ScanAsync(Action<bool> ScanOver)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    BluetoothRadio BuleRadio = BluetoothRadio.PrimaryRadio;
                    BuleRadio.Mode = RadioMode.Connectable;
                    BluetoothDeviceInfo[] Devices = blueClient.DiscoverDevices();
                    deviceAddress.Clear();
                    foreach (BluetoothDeviceInfo device in Devices)
                    {
                        deviceAddress[device.DeviceName] = device.DeviceAddress;
                    }
                    if (ScanOver != null)
                    {
                        ScanOver(true);
                    }
                }
                catch(Exception ex)
                {
                    if (ScanOver != null)
                    {
                        ScanOver(false);
                    }
                }
                
            });
        }
        /// <summary>
        /// 连接蓝牙
        /// </summary>
        /// <param name="address">蓝牙设备地址</param>
        /// <param name="pwd">连接密码</param>
        /// <returns></returns>
        public bool Connect(BluetoothAddress address, string pwd)
        {
            try
            {
                blueClient.SetPin(address, pwd);
                blueClient.Connect(address, BluetoothService.SerialPort);
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            blueClient.Close();
        }
        /// <summary>
        /// 打印文本
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        public bool PrintString(string mess,out int error)
        {
            error = 0;
            if (blueClient.Connected)
            {
                byte[] OutBuffer;//数据
                int BufferSize;
                Encoding targetEncoding;
                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(mess); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(mess);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码
                blueClient.Client.Send(OutBuffer);
                
                return true;
            }
            else
            {
                error =(int)BluetoothErrorEnum.NotConnected;
                return false;
            }
        }
        /// <summary>
        /// 打印图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public bool PrintImg(Bitmap bitmap,out int error)
        {
            error = 0;
            if (blueClient.Connected)
            {
                byte[] data = Pos.POS_PrintPicture(bitmap, 384, 0);
                blueClient.Client.Send(data);
                return true;
            }
            else
            {
                error = (int)BluetoothErrorEnum.NotConnected;
                return false;
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
            if (blueClient != null)
                blueClient.Close();
            disposed = true;
        }
        ~BluetoothHelper()
        {
            Dispose(false);
        }
    }
}
