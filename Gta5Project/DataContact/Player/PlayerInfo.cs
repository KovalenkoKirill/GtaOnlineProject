using GtaServer.DataContact;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{
    [ProtoContract]
    [DataType(PacketType.playerInfo)]
    public class PlayerInfo
    {
        [ProtoMember(1)]
        public Vector3 Position { get; set; }

        [ProtoMember(2)]
        public int Health { get; set; }

        [ProtoMember(3)]
        public int VehicleHealth { get; set; }

        [ProtoMember(4)]
        public PlayerFlags flags { get; set; }

        [ProtoMember(5)]
        public decimal Money { get; set; }
    }
}
