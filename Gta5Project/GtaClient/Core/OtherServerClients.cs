using DataContact;
using GtaServer.DataContact;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient.Core
{
    public class OtherServerClients
    {
        const int PedUpdateTimeOut = 3000;

        public string Session { get; set; }

        public Player Player { get; set; }

        public IPAddress Ip { get; set; }

        public byte[] hash { get; set; }

        public int port { get; set; }

        public IPEndPoint endPoint { get; set; }

        List<SyncPed> SyncPeds = new List<SyncPed>();

        public OtherServerClients(DiscoveryNearPlayer discovery)
        {
            Ip = new IPAddress(discovery.Ip);
            Player = discovery.Player;
            hash = discovery.Session;
            port = discovery.port;
            AddPedInfo(discovery.Player.PedInfo);
            Session = Convert.ToBase64String(hash);
            endPoint = new IPEndPoint(Ip, port);
        }

        public void RenderAll()
        {
            foreach(var sync in SyncPeds.ToList())
            {
                if(sync._lastUpdatePedInfoTime.AddMilliseconds(PedUpdateTimeOut)<DateTime.Now)
                {
                    sync.Remove();
                    SyncPeds.Remove(sync);
                    continue;
                }
                sync.Sync();
            }
        }

        public void AddPedInfo(PedInfo ped)
        {
            if(SyncPeds.Where(x => x.PedId == ped.PedId).Count() == 0)
            {
                SyncPed sync = new SyncPed(ped);
                SyncPeds.Add(sync);
            }
            else
            {
                SyncPeds.Where(x => x.PedId == ped.PedId).Single().PedInfo = ped;
            }
        }

        public void CreateResponse<T>(T data,PacketType type)
        {
            StandardPackage<T> package = new StandardPackage<T>(data, hash, type);
            NetOutgoingMessage responseMessage = MainClient.Instance.Client.CreateMessage();
            byte[] responseBytes = package.Serialize();
            responseMessage.Data = responseBytes;
            responseMessage.LengthBytes = responseBytes.Length;
            MainClient.Instance.Client.SendUnconnectedMessage(responseMessage, endPoint);
        }

        public void RemoveAllPeds()
        {
            GameTaskFactory.Factory.Invoke(() => {
                foreach(var sync in SyncPeds)
                {
                    sync.Remove();
                }
            });
        }
    }
}
