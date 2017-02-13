using GtaServer.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGtaProject.forServer
{
    public class TestIlogger : ILogger
    {
        public void Exception(string message, Exception exception)
        {
            Write($@"Exception : {message}
                    Message:{exception.Message},
                    StackTrace:{exception.StackTrace}
");
        }

        public void Info(string message)
        {
            Write($"Info : {message}");
        }

        public void Trace(string message)
        {
            Write($"Trace : {message}");
        }

        private void Write(string message)
        {
            Debug.WriteLine($"{DateTime.Now.TimeOfDay.ToString()}: {message}");
        }
    }
}
