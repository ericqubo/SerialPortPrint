using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortPrint
{
    /// <summary>
    /// 打印机队列管理
    /// </summary>
    internal static class PrintQueue
    {
        /// <summary>
        /// 打印队列
        /// </summary>
        public static Queue<byte[]> QueueList = new Queue<byte[]>();   
    }
}
