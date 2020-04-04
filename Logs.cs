using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PI_Connect
{
    class Logs
    {

        private string fileName;
        public void LogFile()
        {
            fileName = "C:\\Durai_Projects\\Projects\\PI_Connect\\PI_Connect\\Logs.txt";
        }
        public void LogFile(string fileName)
        {
            this.fileName = fileName;
        }
    
        public void MyLogFile(Exception ex)
        {
            // Store the script names and test results in a output text file.
            using (StreamWriter writer = new StreamWriter(new FileStream("C:\\Durai_Projects\\Projects\\PI_Connect\\PI_Connect\\Logs.txt", FileMode.Append)))
            {
                writer.WriteLine("=============Error Logging ===========");
                writer.WriteLine("===========Start============= " + DateTime.Now);
                writer.WriteLine("Error Message: " + ex.Message);
                writer.WriteLine("Stack Trace: " + ex.StackTrace);
                writer.WriteLine("===========End============= " + DateTime.Now);
            }
        }
    }
}
