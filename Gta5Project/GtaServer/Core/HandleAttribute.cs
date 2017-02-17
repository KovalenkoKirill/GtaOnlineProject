using GtaServer.DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer
{
    public class HandleAttribute:Attribute
    {
        public PacketType type { get; private set; }

        public HandleAttribute(PacketType type)
        {
            this.type = type;
        }
    }
}
