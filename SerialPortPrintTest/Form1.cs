
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

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        PrintHelper print = new PrintHelper();
        public Form1()
        {
            InitializeComponent();
            string err = "";
            if (!print.PrintInit(out err)) {
                MessageBox.Show(err);
            }
            print.PrintCallback = (state) =>
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

        private void button1_Click(object sender, EventArgs e)
        {
            string mes = "";
            if (!print.PrintString("12345679 你好你好你在哪里你是谁 www.mallcoo.cn maokumeishi 毛裤点菜APP", out mes)) {
                MessageBox.Show(mes);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string mes = "";
            string imgPath = "img/3.jpg";
            Bitmap bitmap = new Bitmap(imgPath);
            if (!print.PrintImg(bitmap, out mes)) {
                MessageBox.Show(mes);
            }
        }
    }
}
