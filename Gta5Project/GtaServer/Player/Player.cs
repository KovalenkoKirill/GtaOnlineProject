using DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer
{
    public class Player
    {
        public string Name { get; internal set; }

        public string DisplayName { get; internal set; }

        public float Latency { get; internal set; }

        public Vector3 Position { get; internal set; }

        public int Health { get; internal set; }

        public int VehicleHealth { get; internal set; }

        public PlayerFlags flags { get; internal set; }
    }
}
