using DataContact;
using DataContact.Authorization;
using GtaServer.Configuration;
using GtaServer.interfaces;
using GtaServer.Modules;
using Lidgren.Network;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GtaServer
{
    public enum ServerState:byte
    {
        Creating,
        Starting,
        Started,
        Stoping,
        Stoped,
        Disposed
    }

    public class Server:IDisposable
    {
        #region Property

        public static Server _instanse;

        public static Server Instanse
        {
            get
            {
                return null;
            }
        }

        public ServerState State { get; private set; }

        internal Hashtable clients { get; set; }

        internal IConfiguration configuration;

        private NetServer _server;

        private volatile bool IsWork = false;

        Thread _getNetworkBufferThread = null;

        public GameMode Mode { get; private set; }

        private IAuthorization Authorization = new Authorization();

        #endregion Property

        #region Events

        public event OnClientConnection OnClientConnection;

        #endregion

        #region init

        public Server(IConfiguration configuration,GameMode mode = GameMode.Basic)
        {
            if (Instanse != null) Instanse.Dispose();
            _instanse = this;
            this.Mode = mode;
            this.State = ServerState.Creating;
            this.configuration = configuration;
            configuration.logger.Trace("Creating server object");
            ThreadPool.SetMaxThreads(configuration.maxThread, configuration.maxThread);
        }
        #endregion
        #region Service
        public void Start()
        {
            this.State = ServerState.Starting;
            configuration.logger.Trace("Server starting...");
            NetPeerConfiguration config = new NetPeerConfiguration("GTAVOnline");
            config.Port = this.configuration.serverPort;
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            clients = new Hashtable();
            _server = new NetServer(config);
            _server.Start();
            configuration.logger.Trace($"Server started on {config.Port} port");
            this.IsWork = true;
            _getNetworkBufferThread = new Thread(GetNetworkBuffer);
            this.State = ServerState.Started;
        }
        public void Stop()
        {
            this.State = ServerState.Stoping;
            _server.Shutdown("Server Disconect");
            this.IsWork = false;
            _getNetworkBufferThread.Abort();
            _getNetworkBufferThread = null;
            this.State = ServerState.Stoped;
        }
        public void Restart()
        {
            Stop();
            Start();
        }
        public void Dispose()
        {
            this.Stop();
            configuration.logger.Trace($"Server object is Dispose");
            this.State = ServerState.Disposed;
        }
        #endregion


        private void GetNetworkBuffer()
        {
            while(true)
            {
                List<NetIncomingMessage> messages = new List<NetIncomingMessage>(100);
                int messagesCount = _server.ReadMessages(messages);
                foreach(var message in messages)
                {
                    switch(message.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            Task.Factory.StartNew(() =>{
                                StandardPackage<AuthorizationRequest> package = StandardPackage<AuthorizationRequest>.GetStandardPackage(message.Data);
                                Player player = Authorization.Autorize(package.data.Name, package.data.Password);
                                StandardPackage<AuthorizationResponse> response;
                                if (player != null)
                                {
                                    Client client = new Client(message.SenderConnection, player);
                                    this.clients.Add(client.Session, clients);
                                    response = new StandardPackage<AuthorizationResponse>(
                                        new AuthorizationResponse()
                                        {
                                            Session = client.Session,
                                            Success = true
                                        }, client._session);
                                    this.OnClientConnection(client);
                                }
                                else
                                {
                                    response = new StandardPackage<AuthorizationResponse>(
                                        new AuthorizationResponse()
                                        {
                                            Session = "",
                                            Success = false,
                                            Error = "wrong username or password"
                                        }, null);
                                }
                                NetOutgoingMessage responseMessage = _server.CreateMessage();
                                responseMessage.Data = response.Serialize();
                                message.SenderConnection.SendMessage(responseMessage, NetDeliveryMethod.ReliableOrdered, 0);
                            });
                            break;
                    }
                }
                Thread.Sleep(1);
            }
        }
    }
}
