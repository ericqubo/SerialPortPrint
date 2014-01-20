using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class Printer
    {
        SerialPort server = new SerialPort();
        private string _portName = "COM4";//蓝牙一般默认为com6



        /// <summary>
        /// 获取或设置端口名称
        /// </summary>
        public string PortName
        {
            get
            {
                _portName = server.PortName;
                return _portName;
            }
            set
            {
                _portName = value;
                server.PortName = _portName;
            }
        }
        /// <summary>
        /// 端口是否已经打开
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return server.IsOpen;
            }
        }
        /// <summary>
        /// 构造方法初始化串口参数
        /// </summary>
        public Printer()
        {//初始化各个参数
            server.BaudRate = 9600;//波特率
            server.Parity = 0;//校检位
            server.DataBits = 8;//数据位
            server.StopBits = StopBits.One;//停止位
            server.PortName = _portName;//端口名称
            server.WriteTimeout = -1;//超时时间
            server.ReadTimeout = -1;//超时时间
            server.DataReceived += server_DataReceived;
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

            if (result == 96)
            {

            }
            else if (result == 108)
            {

            }
        }
        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        public bool OpenPort()
        {
            try
            {
                if (!server.IsOpen)
                {//关闭的
                    MessageBox.Show(server.PortName);
                    server.Open();
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool SendDataToPort(string str, out string mes)
        {
            try
            {
                byte[] OutBuffer;//数据

                int BufferSize;
                Encoding targetEncoding;


                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(str); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(str);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码

                byte[] cmdData = new byte[BufferSize + 100];

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

                server.Write(cmdData, 0, 5);
                //向打印机发送[GB码]数据
                server.Write(cmdData, 5, BufferSize + 5);
                //server.WriteLine(str);
                //初始化指令1B 40
                cmdData[0] = 0x1B;
                cmdData[1] = 0x40;
                //打印并走纸指令
                cmdData[2] = 0x1B;
                cmdData[3] = 0x64;
                cmdData[4] = 0x02;

                server.Write(cmdData, 0, 5);
                mes = "";
                return true;
            }
            catch (Exception ex)
            {
                mes = ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool SendDataToPortOld(string str, out string mes)
        {
            try
            {

                byte[] OutBuffer;//数据

                int BufferSize;
                Encoding targetEncoding;

                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(str); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(str);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码


                byte[] cmdDataInit = new byte[5];

                //初始化打印机
                //cmdDataInit[0] = 0x1B;
                //cmdDataInit[1] = 0x40;
                server.Write(cmdDataInit, 0, 5);

                //int r0 = server.ReadChar();

                //设置字符顺时旋转180度
                //cmdData[2] = 0x1B;
                //cmdData[3] = 0x56;
                //cmdData[4] = 0;
                byte[] cmdData = new byte[BufferSize + 100];

                for (int i = 0; i < BufferSize; i++)
                {
                    cmdData[5 + i] = OutBuffer[i];
                }


                cmdDataInit[0] = 0x1D;
                cmdDataInit[1] = 0x72;
                cmdDataInit[2] = 0x01;
                server.Write(cmdDataInit, 0, 3);

                //向打印机发送[GB码]数据
                server.Write(cmdData, 0, BufferSize + 5);



                //int r1 = server.ReadChar();

                //MessageBox.Show("den");

                cmdDataInit[0] = 0x1D;
                cmdDataInit[1] = 0x72;
                cmdDataInit[2] = 0x01;
                server.Write(cmdDataInit, 0, 3);

                // server.Write(cmdData, 0, BufferSize + 5);


                //int r2 = server.ReadChar();



                //sr.Receive();
                //server.WriteLine(str);

                //初始化指令1B 40
                cmdData[0] = 0x1B;
                cmdData[1] = 0x40;
                //打印并走纸指令
                cmdData[2] = 0x1B;
                cmdData[3] = 0x64;
                cmdData[4] = 0x02;

                server.Write(cmdData, 0, 5);
                //cmdData[0] = 0x1D;
                //cmdData[1] = 0x72;
                //cmdData[2] = 0x01;

                //server.Write(cmdData, 0, 3);
                //int r2 = server.ReadChar();
                //sr.Receive();


                mes = "";
                //ClosePort();
                return true;
            }
            catch (Exception ex)
            {
                mes = ex.Message;
                return false;
            }
        }
        /// <summary>
        /// 发送图片数据
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool SendImgDataToPort(string imgPath, out string mes)
        {
            try
            {
                byte[] OutBuffer;//数据

                int BufferSize;
                //Encoding targetEncoding;

                /*
                //将[UNICODE编码]转换为[GB码]，仅使用于简体中文版mobile
                targetEncoding = Encoding.GetEncoding(0);    //得到简体中文字码页的编码方式，因为是简体中文操作系统，参数用0就可以，用936也行。
                BufferSize = targetEncoding.GetByteCount(str); //计算对指定字符数组中的所有字符进行编码所产生的字节数           
                OutBuffer = new byte[BufferSize];
                OutBuffer = targetEncoding.GetBytes(str);       //将指定字符数组中的所有字符编码为一个字节序列,完成后outbufer里面即为简体中文编码
                //byte[] cmdData = new byte[BufferSize + 100];
                */
                //OutBuffer = GetImage(imgPath, out BufferSize);




                Bitmap bitmap = new Bitmap(imgPath);
                byte[] imgs = BtManager.Pos.POS_PrintPicture(bitmap, 384, 0);
                byte[] cmdData = new byte[imgs.Length + 11];
                //初始化打印机
                cmdData[0] = 0x1B;
                cmdData[1] = 0x40;
                //设置字符顺时旋转180度
                cmdData[2] = 0x1B;
                cmdData[3] = 0x56;
                cmdData[4] = 0;

                cmdData[5] = 0x1B;
                cmdData[6] = 0x2A;
                cmdData[7] = 0x32;
                cmdData[8] = 0x20;
                cmdData[9] = 0x2;
                cmdData[10] = 0x50;



                for (int i = 0; i < imgs.Length; i++)
                {
                    cmdData[11 + i] = imgs[i];
                }
                //向打印机发送[GB码]数据
                server.Write(cmdData, 0, cmdData.Length);

                //初始化指令1B 40
                cmdData[0] = 0x1B;
                cmdData[1] = 0x40;
                //打印并走纸指令
                cmdData[2] = 0x1B;
                cmdData[3] = 0x64;
                cmdData[4] = 0x02;

                server.Write(cmdData, 0, 5);
                mes = "";
                return true;
            }
            catch (Exception ex)
            {
                mes = ex.Message;
                return false;
            }
        }
        public void ClosePort()
        {
            server.Close();
        }
    }
}
