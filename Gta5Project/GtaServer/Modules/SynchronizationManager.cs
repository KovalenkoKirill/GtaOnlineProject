using DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GtaServer.Modules
{
    public class SynchronizationManager: IDisposable
    {
        const int interval = 20;

        volatile bool IsWork = false;

        Thread workThred;

        public void Dispose()
        {
            this.IsWork = false;
            try
            {
                workThred.Abort();
            }
            catch { }
        }

        private void SyncPlayers()
        {
            while(IsWork)
            {
                List<Client> clientList = Server.Instanse.clients.ToList();
                foreach (var client in clientList)
                {
                    if(client.lastUpdateTime.AddMilliseconds(interval)<=DateTime.Now)
                    {
                        UpdateSyncDataForPlayer(client, clientList);
                        client.lastUpdateTime = DateTime.Now;
                    }
                }
                Thread.Sleep(1);
            }
        }

        private void UpdateSyncDataForPlayer(Client client, List<Client> clientList)
        {
            foreach (var otherClient in clientList)
            {
                try
                {
                    if (otherClient == client) continue;
                    if(client.NetConnection.Status == Lidgren.Network.NetConnectionStatus.Disconnected)
                    {
                        Server.Instanse.DisconnectClient(client);
                        break;
                    }
                    if(otherClient.NetConnection.Status == Lidgren.Network.NetConnectionStatus.Disconnected)
                    {
                        Server.Instanse.DisconnectClient(otherClient);
                        continue;
                    }
                    float distanse = Utility.Distanse(client.Player.player.PedInfo.Position, otherClient.Player.player.PedInfo.Position);
                    if (distanse < 200f)
                    {
                        if (!client.NearPlayers.Contains(otherClient))
                        {
                            if (!otherClient.NearPlayers.Contains(client)) otherClient.NearPlayers.Add(client);
                            byte[] session = Utility.GetSessionToken();
                            client.NearPlayers.Add(otherClient);
                            DiscoveryNearPlayer nearPlayers = new DiscoveryNearPlayer()
                            {
                                Ip = otherClient.NetConnection.RemoteEndPoint.Address.GetAddressBytes(),
                                Player = otherClient.Player.player,
                                port = otherClient.NetConnection.RemoteEndPoint.Port,
                                Session = session

                            };
                            DiscoveryNearPlayer nearForOther = new DiscoveryNearPlayer()
                            {
                                Ip = client.NetConnection.RemoteEndPoint.Address.GetAddressBytes(),
                                Player = client.Player.player,
                                port = client.NetConnection.RemoteEndPoint.Port,
                                Session = session
                            };
                            otherClient.Send(nearForOther,Lidgren.Network.NetDeliveryMethod.ReliableUnordered);
                            client.Send(nearPlayers, Lidgren.Network.NetDeliveryMethod.ReliableUnordered);
                            Server.Instanse.Logger.Info($"Client {client.Player.Login} is near {otherClient.Player.Login}");
                        }
                    }
                    else
                    {
                        if (client.NearPlayers.Contains(otherClient))
                        {
                            client.NearPlayers.Remove(otherClient);
                            PlayerDisconnect disconect = new PlayerDisconnect()
                            {
                                PlayerId = client.Player.player.PlayerId
                            };
                            otherClient.Send(disconect, Lidgren.Network.NetDeliveryMethod.ReliableOrdered, 1);
                            disconect = new PlayerDisconnect()
                            {
                                PlayerId = otherClient.Player.player.PlayerId
                            };
                            client.Send(disconect, Lidgren.Network.NetDeliveryMethod.ReliableOrdered, 1);
                            Server.Instanse.Logger.Info($"Client {client.Player.Login} to far {otherClient.Player.Login}");
                        }
                    }
                }catch(Exception ex)
                {
                    Server.Instanse.Logger.Exception("", ex);
                }
            }

        }

        public void Start()
        {
            workThred = new Thread(SyncPlayers);
            IsWork = true;
            workThred.Start();
        }
    }
}
