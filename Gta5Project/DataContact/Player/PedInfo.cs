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
    [DataType(PacketType.pedInfo)]
    public class PedInfo
    {
        [ProtoMember(1)]
        public LVector3 Position { get; set; }

        [ProtoMember(2)]
        public int Health { get; set; }

        [ProtoMember(3)]
        public int VehicleHealth { get; set; }

        [ProtoMember(4)]
        public PedFlags flags { get; set; }

        [ProtoMember(5)]
        public decimal Money { get; set; }

        [ProtoMember(6)]
        public uint PedId { get; set; }

        [ProtoMember(7)]
        public uint PedModelHash { get; set; }

        [ProtoMember(8)]
        public uint WeaponHash { get; set; }

        [ProtoMember(9)]
        public LQuaternion Quaternion { get; set; }

        [ProtoMember(10)]
        public LVector3 AimCoords { get; set; }
    }
}
