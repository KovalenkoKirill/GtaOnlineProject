using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{
    [ProtoContract]
    public class Player
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string DisplayName { get; set; }

        [ProtoMember(3)]
        public Vector3 Position { get; set; }

        [ProtoMember(4)]
        public int Health { get; set; }

        [ProtoMember(5)]
        public int VehicleHealth { get; set; }

        [ProtoMember(6)]
        public PlayerFlags flags { get; set; }
    }
}
