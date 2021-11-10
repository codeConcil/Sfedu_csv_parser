using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSVParser
{
    class LogParserProgress
    {
        const string directory = "\\CSVparser\\Logs";
        public static string CurrentLogFilePath { get; private set; }
        public static void Log(string value, string logFilePath = null)
        {
            if (logFilePath == null)
                logFilePath = CurrentLogFilePath;

            if (!string.IsNullOrEmpty(logFilePath))
            {
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine(value);
                }
            }
        }

        public static string GetNewLogFilePath(string name)
        {
            CurrentLogFilePath = Path.Combine(GetAppDirectory(), 
                name + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + ".txt");
            return CurrentLogFilePath;
        }

        public static string GetAppDirectory()
        {
            string dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + directory;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }
    }
}
