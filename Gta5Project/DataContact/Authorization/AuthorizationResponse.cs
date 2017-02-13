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
    [ProtoContract]
    public class AuthorizationResponse
    {
        public bool Success { get; set; }

        public string Session { get; set; }

        public string Error { get; set; }
    }
}
