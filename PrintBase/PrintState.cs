using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintBase
{
    /// <summary>
    /// 打印机状态
    /// </summary>
    public enum PrintState
    {
        /// <summary>
        /// 打印机异常
        /// </summary>
        Error = -1,
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 96,
        /// <summary>
        /// 缺纸
        /// </summary>
        Nopaper = 108


    }
}
