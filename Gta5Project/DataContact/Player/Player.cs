using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{
    [ProtoContract]
    [DataType(GtaServer.DataContact.PacketType.playerInfo)]
    public class Player
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string DisplayName { get; set; }

        [ProtoMember(3)]
        public Guid PlayerId { get; set; }

        [ProtoMember(4)]
        public PedInfo PedInfo { get; set; }
    }
}
