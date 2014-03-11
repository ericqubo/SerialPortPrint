
using PrintBase;
using SerialPortPrint;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test.SerialPortPrint
{
    public partial class Form1 : Form
    {
        static string portName = System.Configuration.ConfigurationManager.AppSettings["port"];
        PrintHelper print1;
        public Form1()
        {
            print1 = new PrintHelper(portName);
            InitializeComponent();

            /*
            Bitmap bitmapOrg=new Bitmap ("");

            Bitmap bmp = new Bitmap(500, 500);
            Graphics g = Graphics.FromImage(bmp);
            g.Transform = new System.Drawing.Drawing2D.Matrix();
            Rectangle rect = new Rectangle(0, 0, 500, 500);
            
            g.FillRectangle(Brushes.White, rect);

            // 改变图像大小使用低质量的模式 
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            g.DrawImage(bitmapOrg, new Rectangle(0, 0, 500, 500), new Rectangle(0, 0, bitmapOrg.Width, bitmapOrg.Height), GraphicsUnit.Point);
            */
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string mes = "";
            if (!print1.PrintString("12345679 你好你好你在哪里你是谁 www.mallcoo.cn maokumeishi 毛裤点菜APP", out mes)) {
                MessageBox.Show(mes);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string mes = "";
            string imgPath = "img/Test.png";
            Bitmap bitmap = new Bitmap(imgPath);
            if (!print1.PrintImg(bitmap, out mes)) {
                MessageBox.Show(mes);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            print1 = new PrintHelper(portName);
            string err = "";
            if (!print1.PrintInit(out err))
            {
                MessageBox.Show(err);
            }
            print1.PrintCallback = (state) =>
            {
                if (state == PrintState.Nopaper)
                {
                    MessageBox.Show("缺纸");
                }
                else if (state == PrintState.Error)
                {
                    MessageBox.Show("打印机异常");
                }
                else if (state == PrintState.Normal)
                {
                    MessageBox.Show("打印机正常");
                }
            };
        }
    }
}
