using GtaClient.Helper.Debug;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient.Logger
{
    public class Debuglogger : ILogger
    {
        public void Exception(string message, Exception exception)
        {
            Write($@"Exception : {message}
                    Message:{exception.Message},
                    StackTrace:{exception.StackTrace}
",ConsoleColor.Red);

        }

        public Debuglogger()
        {
            DebugExtention.ShowConsoleWindow();
        }

        public void Info(string message)
        {
            Write($"Info : {message}", ConsoleColor.Blue);
        }

        public void Trace(string message)
        {
            Write($"Trace : {message}", ConsoleColor.Green);
        }

        private void Write(string message,ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine($"{DateTime.Now.TimeOfDay.ToString()}: {message}");
            Console.ResetColor();
        }
    }
}
