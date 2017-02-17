using DataContact;
using DataContact.Authorization;
using GtaServer;
using GtaServer.Configuration;
using GtaServer.DataContact;
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
using Handlers = System.Collections.Generic.Dictionary<GtaServer.DataContact.PacketType, System.Collections.Generic.ICollection<GtaServer.IHandler>>; // Done


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
                return _instanse;
            }
        }

        public Handlers handlers { get; private set; }

        public ServerState State { get; private set; }

        internal Hashtable clients { get; set; }

        internal IConfiguration configuration;

        private NetServer _server;

        public NetServer NetServer
        {
            get
            {
                return _server;
            }
        }

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
            InizializateHandlers();
        }

        void InizializateHandlers()
        {
            this.handlers = new Handlers();
            foreach(PacketType type in Enum.GetValues(typeof(PacketType)))
            {
                handlers.Add(type, new List<IHandler>());
            }
            var types = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => typeof(IHandler).IsAssignableFrom(p) && p.IsClass);
            foreach(Type type in types)
            {
                object handle = Activator.CreateInstance(type);
                configuration.logger.Trace($"Create {type.Name} as handle of:");
                HandleAttribute [] attributes = (HandleAttribute[])type.GetCustomAttributes(typeof(HandleAttribute),false);
                foreach(var attr in attributes)
                {
                    configuration.logger.Trace($"\t\t{attr.type.ToString()}");
                    this.handlers[attr.type].Add((IHandler)handle);
                }
            }
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
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            clients = new Hashtable();
            _server = new NetServer(config);
            _server.Start();
            configuration.logger.Trace($"Server started on {config.Port} port");
            this.IsWork = true;
            _getNetworkBufferThread = new Thread(GetNetworkBuffer);
            _getNetworkBufferThread.Start();
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
            configuration.logger.Trace("GetNetworkBuffer thread started");
            while (true)
            {
                List<NetIncomingMessage> messages = new List<NetIncomingMessage>(100);
                int messagesCount = _server.ReadMessages(messages);
                foreach(var message in messages)
                {
                    switch(message.MessageType)
                    {
                        case NetIncomingMessageType.Data:
                            this.configuration.logger.Trace($"Data from {message.SenderEndPoint.Address.ToString()}");
                            StandardPackage<object> package = StandardPackage<object>.GetStandardPackage(message.Data);
                            string session = package.Session;
                            if (!this.clients.ContainsKey(session))
                            {
                                message.SenderConnection.Deny("Client not found. Update Session");
                                continue;
                            }
                            Client client = (Client)this.clients[session];
                            if (client == null) continue;
                            foreach(IHandler handle in handlers[package.type])
                            {
                                handle.HandlePackage(package, client);
                            }
                            break;
                        case NetIncomingMessageType.ConnectionApproval:
                            this.configuration.logger.Trace($"Data from {message.SenderEndPoint.Address.ToString()}");
                            Task.Factory.StartNew(() => {
                                AuthorizationHandler(message);
                            });
                            break;
                        case NetIncomingMessageType.WarningMessage:
                            string Data = Encoding.UTF8.GetString(message.Data);
                            this.configuration.logger.Trace($"{message.MessageType.ToString()} from {message.SenderEndPoint?.Address.ToString()}: Data {Data}");
                            break;

                    }
                }
                Thread.Sleep(1);
            }
        }

        public NetOutgoingMessage CreateResponse<T>(T response,byte [] hash)
        {
            StandardPackage<T> package = new StandardPackage<T>(response, hash);
            NetOutgoingMessage responseMessage = _server.CreateMessage();
            byte[] responseBytes = package.Serialize();
            responseMessage.Data = responseBytes;
            responseMessage.LengthBytes = responseBytes.Length;
            return responseMessage;
        }
        public NetOutgoingMessage CreateResponse<T>(T response, byte[] hash,PacketType type)
        {
            StandardPackage<T> package = new StandardPackage<T>(response, hash, type);
            NetOutgoingMessage responseMessage = _server.CreateMessage();
            byte[] responseBytes = package.Serialize();
            responseMessage.Data = responseBytes;
            responseMessage.LengthBytes = responseBytes.Length;
            return responseMessage;
        }

        private void AuthorizationHandler(NetIncomingMessage message)
        {
            try
            {
                StandardPackage<AuthorizationRequest> package = StandardPackage<AuthorizationRequest>.GetStandardPackage(message.Data);
                if(package.data == null)
                {
                    message.SenderConnection.Deny("Package is null");
                    return;
                }
                ServerPlayer player = Authorization.Autorize(package.data.Name, package.data.Password);
                if (player != null)
                {
                    Client client = new Client(message.SenderConnection, player);
                    this.clients.Add(client.Session, client);
                    NetOutgoingMessage responseMessage = CreateResponse(
                        new AuthorizationResponse()
                        {
                            Session = client.Session,
                            Success = true,
                            Player = player
                        }, client._session);
                    message.SenderConnection.Approve(responseMessage);
                    this.OnClientConnection?.Invoke(client);
                }
                else
                {
                    message.SenderConnection.Deny("wrong username or password");
                }
            }
            catch (Exception e)
            {
                this.configuration.logger.Exception(e.Message, e);
            }
        }
    }
}
