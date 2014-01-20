using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialPortPrint
{
    public static class Log
    {
        public static void Error(Exception ex)
        {
            SaveLog("error.txt", ex.StackTrace);
        }

        public static void Debug(string info)
        {
            SaveLog("error.txt", info);
        }
        private static void SaveLog(string fileName, string message)
        {
            message = "\r\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "-----------------------------------\r\n" + message;
            //string path = ConfigHelp.ProjectPath + DateTime.Now.ToString("yyyy/MM/dd/");

            string path = "Log/" + DateTime.Now.ToString("yyyy/MM/dd/");

            path = System.IO.Path.GetFullPath(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = path + fileName;
            File.AppendAllText(path, message);
        }
    }
}
