using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer.DataContact
{
    public enum PacketType : byte
    {
        ALL,
        unknown,
        authorization,
        playerInfo,
        pedInfo,
        nearPlayer,
        playerToFar,
        playerDisconnect
    }
}
