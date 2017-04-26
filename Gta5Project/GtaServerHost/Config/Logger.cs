using GtaServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServerHost.Config
{
    public class Logger : ILogger
    {
        public void Exception(string message, Exception exception)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine($"Exception: {message} \r\n{exception.Message} \r\n {exception.StackTrace}");
            Console.ResetColor();
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"Info: {message}");
            Console.ResetColor();
        }

        public void Trace(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Trace: {message}");
            Console.ResetColor();
        }
    }
}
