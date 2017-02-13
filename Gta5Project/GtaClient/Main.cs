using DataContact.Authorization;
using DataContact;
using GTA;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameVersion = DataContact.GameVersion;

namespace GtaClient
{
    public class MainClient: Script
    {
        public ClientConfiguration Configuration { get; private set; }

        public NetClient Client { get; private set; }

        public MainClient(ClientConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public void Connect()
        {
            NetPeerConfiguration connectionConfig = new NetPeerConfiguration("GTAVOnline");
            connectionConfig.AutoExpandMTU = true;
            connectionConfig.Port = GetOpenUdpPort();
            connectionConfig.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            Client = new NetClient(connectionConfig);
            Client.Start();
            Client.Connect(Configuration.ServerAdress, Configuration.ServerPort);
        }
        private int GetOpenUdpPort()
        {
            var startingAtPort = 5000;
            var maxNumberOfPortsToCheck = 500;
            var range = Enumerable.Range(startingAtPort, maxNumberOfPortsToCheck);
            var portsInUse =
                from p in range
                join used in System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners()
            on p equals used.Port
                select p;

            return range.Except(portsInUse).FirstOrDefault();
        }

        public void SendPackage<T>(StandardPackage<T> package,NetDeliveryMethod deliveryMethod = NetDeliveryMethod.UnreliableSequenced)
        {
            NetOutgoingMessage message = Client.CreateMessage();
            message.Data = package.Serialize();
            Client.SendMessage(message, deliveryMethod);
        }

        public void Autorization(string login,string password)
        {
            AuthorizationRequest request = new AuthorizationRequest()
            {
                Name = login,
                Password = password,
                GameVersion = ((GameVersion)(byte)GTA.Game.Version)
            };
            StandardPackage<AuthorizationRequest> requestPack = new StandardPackage<AuthorizationRequest>(request, null);
            SendPackage(requestPack);
        }
    }
}
