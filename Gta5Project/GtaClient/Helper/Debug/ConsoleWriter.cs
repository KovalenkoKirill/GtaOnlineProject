using GTA;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient.Helper.Debug
{

    public class ConsoleWriter : Script
    {
        Ped ped;
        public ConsoleWriter()
        {
            Tick += OnTick;
            Interval = 100;
            ped = GTA.World.CreateRandomPed(Game.Player.Character.Position.Around(1));
#if DEBUG
            DebugExtention.ShowConsoleWindow();
#endif
        }

        readonly bool[] _active = new bool[2];
        readonly DateTime[] _timeLeft = new DateTime[2];

        void OnTick(object sender, EventArgs e)
        {
            Console.Clear();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (!ped.IsRunning && !ped.IsWalking)
            {
                ped.Task.GoTo(Game.Player.Character.Position);
            }

            watch.Stop();

            Console.WriteLine($"ElapsedMilliseconds:{watch.ElapsedMilliseconds}");
        }
    }
}
