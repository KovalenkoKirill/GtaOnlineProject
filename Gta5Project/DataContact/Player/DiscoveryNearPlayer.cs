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
    [DataType(PacketType.nearPlayer)]
    public class DiscoveryNearPlayer
    {
        [ProtoMember(1)]
        public byte[] Session { get; set; }

        [ProtoMember(2)]
        public Player Player { get; set; }

        [ProtoMember(3)]
        public byte[] Ip { get; set; }

        [ProtoMember(4)]
        public int port { get; set; }
    }
    [ProtoContract]
    [DataType(PacketType.playerToFar)]
    public class PlayerToFarInfo
    {
        [ProtoMember(1)]
        public Guid PlayerId { get; set; }
    }
    [ProtoContract]
    [DataType(PacketType.playerDisconnect)]
    public class PlayerDisconnect
    {
        [ProtoMember(1)]
        public Guid PlayerId { get; set; }
    }
}
