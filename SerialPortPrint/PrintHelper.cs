using System;
using System.Collections.Generic;

using System.IO;
#if ANDROID
using Android.Graphics;
#else
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
#endif

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrintBase;

namespace SerialPortPrint
{
    /// <summary>
    /// 
    /// </summary>
    public class PrintHelper
    {

        SerialPort server = new SerialPort();
        bool isPrintOk = false;
        //private string _portName = "COM6";
        /// <summary>
        /// 打印状态回调
        /// </summary>
        public Action<PrintState> PrintCallback;

        /// <summary>
        /// 端口是否已经打开
        /// </summary>
        private bool IsOpen
        {
          
            get
            {
                return server.IsOpen;
            }
        }
        /// <summary>
        /// 构造方法初始化串口参数
        /// </summary>
        public PrintHelper(string portName)
        {//初始化各个参数
            server.BaudRate = 9600;//波特率
            server.Parity = 0;//校检位
            server.DataBits = 8;//数据位
            server.StopBits = StopBits.One;//停止位
            server.PortName = portName;//端口名称
            server.WriteTimeout = -1;//超时时间
            server.ReadTimeout = -1;//超时时间


            
        }
        public bool PrintInit(out string err) {
            err = "";
            server.DataReceived += server_DataReceived;
            //打印机检测
           // CheckPrintState(out err);
            if (err != "")
            {
                return false;
            }
            else
            {
                isPrintOk = true;
                return true;
            }
        }
        /// <summary>
        ///此方法没有接收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void server_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort s = sender as SerialPort;

            int result = s.ReadChar();

            if (result == (int)PrintState.Normal)
            {
                if (PrintCallback != null)
                {
                    PrintCallback(PrintState.Normal);
                    //打印数据
                    while (PrintQueue.QueueList.Count() > 0)
                    {
                        byte[] data = PrintQueue.QueueList.Dequeue();
                        string err;
                        SendData(data, out err);
                    }
                }
            }
            else if (result == (int)PrintState.Nopaper)
            {
                if (PrintCallback != null)
                {
                    PrintCallback(PrintState.Nopaper);
                }
            }
            else
            {
                if (PrintCallback != null)
                {
                    PrintCallback(PrintState.Error);
                }
            }
        }
        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        private bool OpenPort(out string err)
        {
            err = "";
            try
            {
                if (!server.IsOpen)
                {//关闭的
                    server.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 请求打印机状态
        /// </summary>
        public void CheckPrintState(out string err)
        {
            err = "";

            byte[] data = new byte[3];
            data[0] = 0x1D;
            data[1] = 0x72;
            data[2] = 0x01;
            SendData(data, out err);
        }

        /// <summary>
        /// 将字符串写入打印队列
        /// </summary>
        /// <param name="mes"></param>
        /// <returns></returns>
        public bool PrintString(string mes, out string err)
        {
            err = "";
            if(!isPrintOk)
            {
                err = "打印机初始化失败，无法正常使用";
                return false;

            }
            if (string.IsNullOrEmpty(mes))
            {
                err = "Mes不能为空";
                return false;
            }
            try
            {
                byte[] OutBuffer;//数据
                int BufferSize;
                Encoding targetEncoding;

                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(mes); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(mes);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码

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
                //PrintQueue.QueueList.Enqueue(cmdData);
                //CheckPrintState(out err);
                SendData(cmdData, out err);
                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 将BitImage对象写入打印队列
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public bool PrintImg(Bitmap bitmap, out string err)
        {
            err = "";
            if (!isPrintOk)
            {
                err = "打印机初始化失败，无法正常使用";
                return false;

            }
            if (bitmap == null)
            {
                err = "bitmap不能为null";
                return false;
            }
            try
            {
                byte[] data = Pos.POS_PrintPicture(bitmap, 384, 0);
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
                //PrintQueue.QueueList.Enqueue(cmdData);
                //CheckPrintState(out err);
                SendData(cmdData, out err);
                return true;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 向打印机发送命令
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool SendData(byte[] data, out string err)
        {
            err = "";
            if (!IsOpen)
            {
                if (!OpenPort(out err))
                {
                    Log.Debug("无法打开端口");
                    return false;
                }
                else
                {
                    try
                    {
                        server.Write(data, 0, data.Length);
                        server.Close();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                        return false;
                    }
                }
            }
            else
            {
                try
                {
                    server.Write(data, 0, data.Length);
                    server.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return false;
                }
            }
        }
    }
}
