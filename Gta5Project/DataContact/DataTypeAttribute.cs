using GtaServer.DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{
    public class DataTypeAttribute :Attribute
    {
        public PacketType Type { get; private set; }

        public DataTypeAttribute(PacketType type)
        {
            this.Type = type;
        }
    }
}
