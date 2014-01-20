using System;
using System.Collections.Generic;
using System.Text;

namespace SerialPortPrint
{

    public enum StopBits
    {
        // 摘要: 
        //     不使用停止位。不支持此值。将 System.IO.Ports.SerialPort.StopBits 属性设置为 System.IO.Ports.StopBits.None
        //     将引发 System.ArgumentOutOfRangeException。
        None = 0,
        //
        // 摘要: 
        //     使用一个停止位。
        One = 1,
        //
        // 摘要: 
        //     使用两个停止位。
        Two = 2,
        //
        // 摘要: 
        //     使用 1.5 个停止位。
        OnePointFive = 3,
    }
}
