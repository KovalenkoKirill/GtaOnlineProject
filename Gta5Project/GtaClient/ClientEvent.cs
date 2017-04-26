using DataContact;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient
{
    public delegate void LoginIn();

    public delegate void StatusChanged(NetConnectionStatus status, string message);

    public delegate void OnPacketReceivered(StandardPackage<object> package);
}
