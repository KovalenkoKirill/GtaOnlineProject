using DataContact;
using GtaServer.DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer
{
    public delegate void OnClientConnection(Client client);

    public delegate void OnClientDisconnect(Client client);

    public delegate Task OnPacketReceivered(PacketType type, Client client,StandardPackage<object> package);
}
