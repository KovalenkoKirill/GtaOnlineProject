using GtaClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataContact;
using GtaServer.DataContact;
using System.Net;

namespace GtaClient
{
    [Handle(PacketType.nearPlayer)]
    public class NewPlayerHandler : IHandler
    {
        public void Dispose()
        {
            
        }

        public Task HandlePackage<T>(StandardPackage<T> package)
        {
            DiscoveryNearPlayer discovery = package.data as DiscoveryNearPlayer;
            IPAddress address = new IPAddress(discovery.Ip);
            MainClient.Instance.Logger.Info($"new Player discovery {address.ToString()} {discovery.port}");
            OtherServerClients client = new OtherServerClients(discovery);
            MainClient.Instance.sync.otherClient.Add(client);
            return null;
        }
    }
    [Handle(PacketType.playerToFar)]
    public class PlayerToFarHandler : IHandler
    {
        public void Dispose()
        {

        }

        public Task HandlePackage<T>(StandardPackage<T> package)
        {
            PlayerToFarInfo discovery = package.data as PlayerToFarInfo;
            MainClient.Instance.Logger.Info($"Player {discovery.PlayerId} to far");
            OtherServerClients farClients = MainClient.Instance.sync.otherClient
                .Where(x => x.Player.PlayerId == discovery.PlayerId).SingleOrDefault();
            MainClient.Instance.sync.otherClient.Remove(farClients);
            farClients.RemoveAllPeds();
            return null;
        }
    }
}
