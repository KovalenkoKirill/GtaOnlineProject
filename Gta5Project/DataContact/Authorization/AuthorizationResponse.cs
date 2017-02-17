using GtaServer.DataContact;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact.Authorization
{
    [DataType(PacketType.authorization)]
    [ProtoContract(UseProtoMembersOnly =false)]
    public class AuthorizationResponse
    {
        [ProtoMember(1)]
        public bool Success { get; set; }

        [ProtoMember(2)]
        public string Session { get; set; }

        [ProtoMember(3)]
        public string Error { get; set; }

        [ProtoMember(4)]
        public Player Player { get; set; }
    }
}
