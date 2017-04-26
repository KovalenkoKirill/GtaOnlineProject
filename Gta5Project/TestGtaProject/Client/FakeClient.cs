using DataContact;
using DataContact.Authorization;
using GtaServer.Configuration;
using GtaServer.DataContact;
using Lidgren.Network;
using System;
using System.Collections.Generic;
#if DEBUG
using System.Diagnostics;
#endif
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestGtaProject
{
    public class ClientConfiguration
    {
        public string ServerAdress { get; set; }

        public int ServerPort { get; set; }
    }

    public delegate void StatusChanged(NetConnectionStatus status, string message);

#if TEST
    public class FakeClient 
    {
#else
     public class MainClient: Script
    {
#endif
        public ClientConfiguration Configuration { get; private set; }

        public NetClient Client { get; private set; }

        public Player Player { get; private set; }

        public float Latency { get; private set; }

        public event StatusChanged StatusChanged;

        public bool isAuth { get; private set; }

        public string Session { get; private set; }

        public int clientPort = -1;

        public byte [] SessionAsByte { get; private set; }

        Thread ProcessMessagesThread;

        public ILogger Logger { get; private set; }

        public FakeClient(ClientConfiguration configuration,ILogger logger)
        {
            this.Logger = logger;
            this.Configuration = configuration;
            isAuth = false;
        }

        public void Connect()
        {
            NetPeerConfiguration connectionConfig = new NetPeerConfiguration("GTAVOnline");
            connectionConfig.AutoExpandMTU = true;
            connectionConfig.Port = clientPort == -1 ? GetOpenUdpPort(): clientPort;
            connectionConfig.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            connectionConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            connectionConfig.EnableMessageType(NetIncomingMessageType.Data);
            connectionConfig.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            Client = new NetClient(connectionConfig);
            ProcessMessagesThread = new Thread(ProcessMessages);
            ProcessMessagesThread.Start();
            Client.Start();
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

        public void SendPackage<T>(StandardPackage<T> package, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.UnreliableSequenced)
        {
            NetOutgoingMessage message = Client.CreateMessage();
            message.Data = package.Serialize();
            message.LengthBytes = message.Data.Length;
            Client.SendMessage(message, deliveryMethod);
        }

        private void ProcessMessages()
        {
#if DEBUG
            Logger.Trace($"ProcessMessages client Started");
#endif
            while (true)
            {
                List<NetIncomingMessage> messages = new List<NetIncomingMessage>(100);
                int messagesCount = Client.ReadMessages(messages);
                foreach (var message in messages)
                {
                    Logger.Trace($"Client get {message.MessageType}");
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionLatencyUpdated:
                            this.Latency = message.ReadFloat();
                            Logger.Trace($"Client Latency is {this.Latency}");
                            break;
                        case NetIncomingMessageType.StatusChanged:
                            var newStatus = (NetConnectionStatus)message.ReadByte();
                            try
                            {
                                byte[] messageBytes = new byte[message.Data.Length - 1];
                                Array.Copy(message.Data, messageBytes, message.Data.Length - 1);
                                string StatusMessage = Encoding.UTF8.GetString(messageBytes);
                                this.StatusChanged?.Invoke(newStatus, StatusMessage);
                            }catch(Exception ex)
                            {
                                Logger.Exception($"Error !",ex);
                            }
                            switch (newStatus)
                            {
                                case NetConnectionStatus.InitiatedConnect:
                                    Logger.Trace($"Client Connecting...");
                                    break;
                                case NetConnectionStatus.Connected:
                                    Logger.Trace($"Client Connection successful!");
                                    Autorize(message);
                                    break;
                                case NetConnectionStatus.Disconnected:
                                    Logger.Trace($"Client Disconnected!");
                                    
                                    break;
                            }
                            break;
                    }
                }
                Thread.Sleep(1);
            }
        }

        public NetOutgoingMessage CreateResponse<T>(T response, byte[] hash)
        {
            StandardPackage<T> package = new StandardPackage<T>(response, hash);
            NetOutgoingMessage responseMessage = Client.CreateMessage();
            byte[] responseBytes = package.Serialize();
            responseMessage.Data = responseBytes;
            responseMessage.LengthBytes = responseBytes.Length;
            return responseMessage;
        }
        public NetOutgoingMessage CreateResponse<T>(T response, byte[] hash, PacketType type)
        {
            StandardPackage<T> package = new StandardPackage<T>(response, hash, type);
            NetOutgoingMessage responseMessage = Client.CreateMessage();
            byte[] responseBytes = package.Serialize();
            responseMessage.Data = responseBytes;
            responseMessage.LengthBytes = responseBytes.Length;
            return responseMessage;
        }

        public void Autorization(string login, string password)
        {
            NetOutgoingMessage message = CreateResponse(new AuthorizationRequest()
            {
                Name = login,
                Password = password,
                GameVersion = ((GameVersion)(byte)0)
            },null);
            Client.Connect(Configuration.ServerAdress, Configuration.ServerPort, message);
        }

        PlayerSynchronization sync;

        private void Autorize(NetIncomingMessage message)
        {
            try
            {
                StandardPackage<AuthorizationResponse> auth = StandardPackage<AuthorizationResponse>.GetStandardPackage(message.SenderConnection.RemoteHailMessage.Data);
                if (auth.data.Success)
                {
                    isAuth = true;
                    this.Player = auth.data.Player;
                    this.SessionAsByte = auth.hash;
                    this.Logger.Trace($"Autorization success");
                    sync = new PlayerSynchronization(this);
                }
                else
                {
                    this.Logger.Trace($"Autorization error {auth.data.Error}");
                }
            }
            catch (Exception e)
            {
                this.Logger.Exception($"Client NetIncomingMessageType.ConnectionApproval", e);
            }
        }
    }
}
