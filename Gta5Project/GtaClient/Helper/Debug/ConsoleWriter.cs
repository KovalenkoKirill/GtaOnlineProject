using GTA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient.Helper.Debug
{

    public class ConsoleWriter : Script
    {
        public ConsoleWriter()
        {
            Tick += OnTick;
            Interval = 100;
            using (StreamWriter writer = new StreamWriter("text.txt"))
            {
                writer.Write($@"{DateTime.Now} :Atatch");
            }
#if DEBUG
            DebugExtention.ShowConsoleWindow();
#endif
        }

        readonly bool[] _active = new bool[2];
        readonly DateTime[] _timeLeft = new DateTime[2];

        void OnTick(object sender, EventArgs e)
        {
        }
    }
}
