using GtaServer.DataContact;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact.Authorization
{
    [ProtoContract]
    [DataType(PacketType.authorization)]
    public class AuthorizationRequest
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public GameVersion GameVersion { get; set; }
    }
}
