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
using System.Threading;
using GtaServer.DataContact;
using GtaClient.Logger;
using System.IO;
using Newtonsoft.Json;
using Handlers = System.Collections.Generic.Dictionary<GtaServer.DataContact.PacketType, System.Collections.Generic.ICollection<GtaClient.Core.IHandler>>; // Done
using GtaClient.Core;
#if TEST
#else
#if DEBUG
using CurrentLogger = GtaClient.Logger.Debuglogger;
#else
using CurrentLogger = GtaClient.Logger.EmptyLogger;
#endif
#endif
namespace GtaClient
{
#if TEST
    public class FakeClient 
    {
#else
    public partial class MainClient : Script,IDisposable
    {
#endif
        public ClientConfiguration Configuration { get; private set; }

        public NetClient Client { get; private set; }

        public DataContact.Player Player { get; private set; }

        public float Latency { get; private set; }

        public event StatusChanged StatusChanged;

        public event OnPacketReceivered OnPacketReceivered;

        public PlayerSynchronization sync { get; private set; }

        public Handlers handlers { get; private set; }

        public bool isAuth { get; private set; }

        public string Session { get; private set; }

        public byte[] SessionAsByte { get; private set; }

        Thread ProcessMessagesThread;

        public ILogger Logger { get; private set; }

        public static MainClient Instance { get; private set; }

        protected new int Interval
        {
            get
            {
                return 10;
            }
        }

#if TEST
        public FakeClient(ClientConfiguration configuration, ILogger logger)
        {
            this.Logger = logger;
            this.Configuration = configuration;
            isAuth = false;
        }
#else
        public MainClient()
        {
            
            Instance = this;
            isAuth = false;
            this.Logger = new CurrentLogger();
            InitConfig();
            InizializateHandlers();
            Connect();
            Autorization($"dev{new Random().Next(1,1000)}", "12345");
            Logger.Info("MainClient loaded");
#if DEBUG
            var deb = new ForDebug();
#endif
        }

        public void InitConfig()
        {
            string path = Environment.GetEnvironmentVariable("LauncherPath");

            DirectoryInfo dir = Directory.GetParent(path);

            using (StreamReader reader = new StreamReader($@"{dir.Parent.FullName}\Configuration.json"))
            {
                this.Configuration = JsonConvert.DeserializeObject<ClientConfiguration>(reader.ReadToEnd());
            }
        }
        void InizializateHandlers()
        {
            try
            {
                this.handlers = new Handlers();
                foreach (PacketType type in Enum.GetValues(typeof(PacketType)))
                {
                    handlers.Add(type, new List<IHandler>());
                }
                var types = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(s => s.GetTypes())
                                .Where(p => typeof(IHandler).IsAssignableFrom(p) && p.IsClass);
                foreach (Type type in types)
                {
                    try
                    {
                        object handle = Activator.CreateInstance(type);
                        Logger.Trace($"Create {type.Name} as handle of:");
                        HandleAttribute[] attributes = (HandleAttribute[])type.GetCustomAttributes(typeof(HandleAttribute), false);
                        foreach (var attr in attributes)
                        {
                            Logger.Trace($"\t\t{attr.type.ToString()}");
                            this.handlers[attr.type].Add((IHandler)handle);
                        }
                    }catch(Exception ex)
                    {
                        Logger.Exception($"Exception on trying create handle {type.Name}:",ex);
                    }
                }
            }catch(Exception ex)
            {
                Logger.Exception("InizializateHandlers", ex);
            }
        }
#endif
        public void Disconnect()
        {
            try
            {
                Client.Shutdown("");
                sync.Dispose();
            }
            catch { }
        }

        public void Connect()
        {
            NetPeerConfiguration connectionConfig = new NetPeerConfiguration("GTAVOnline");
            connectionConfig.AutoExpandMTU = true;
            connectionConfig.Port = GetOpenUdpPort();
            connectionConfig.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            connectionConfig.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            connectionConfig.EnableMessageType(NetIncomingMessageType.Data);
            connectionConfig.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            Client = new NetClient(connectionConfig);
            ProcessMessagesThread = new Thread(ProcessMessages);
            ProcessMessagesThread.Start();
            Client.Start();
            Logger.Info("Starting to connect!");
        }
        private int GetOpenUdpPort()
        {
            
            var startingAtPort = new Random().Next(5000, 6000);
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

        public void SendPackage<T>(T obj, NetDeliveryMethod deliveryMethod = NetDeliveryMethod.UnreliableSequenced)
        {
            StandardPackage<T> package = new StandardPackage<T>(obj, this.SessionAsByte);
            SendPackage(package, deliveryMethod);
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
                    try
                    {
                        switch (message.MessageType)
                        {
                            case NetIncomingMessageType.ConnectionLatencyUpdated:
                                this.Latency = message.ReadFloat();
                                Logger.Trace($"Client Latency is {this.Latency}");
                                break;
                            case NetIncomingMessageType.Data:
                            case NetIncomingMessageType.UnconnectedData:
                                message.SkipPadBits();
                                byte[] data = message.ReadBytes(message.LengthBytes);
                                StandardPackage<object> package = StandardPackage<object>.GetStandardPackage(data);
                                if(package == null)
                                {
                                    Logger.Info($"Package is null data.Length:{data.Length}\r\n Message.Lenght: {message.LengthBytes}");
                                }
                                string session = package.Session;
                                foreach (IHandler handle in handlers[package.type].AsEnumerable())
                                {
                                    handle.HandlePackage(package);
                                }
                                OnPacketReceivered?.Invoke(package);
                                break;
                            case NetIncomingMessageType.StatusChanged:
                                var newStatus = (NetConnectionStatus)message.ReadByte();
                                try
                                {
                                    byte[] messageBytes = new byte[message.Data.Length - 1];
                                    Array.Copy(message.Data, messageBytes, message.Data.Length - 1);
                                    string StatusMessage = Encoding.UTF8.GetString(messageBytes);
                                    this.StatusChanged?.Invoke(newStatus, StatusMessage);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Exception($"Error !", ex);
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
                                        Disconnect();
                                        break;
                                }
                                break;
                        }
                    }catch(Exception ex)
                    {
                        Logger.Exception($"Package Exception: {Encoding.UTF8.GetString(message.Data)}", ex);
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
            }, null);
            Client.Connect(Configuration.ServerAdress, Configuration.ServerPort, message);
            Logger.Info($"starting autorization as {login} {password}");
        }

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
                    this.Session = Convert.ToBase64String(SessionAsByte);
                    this.Logger.Trace($"Autorization success");
                    sync = new PlayerSynchronization(this);
                    this.handlers[PacketType.pedInfo].Add(sync);
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

        public void Disconect()
        {
            this.sync.Dispose();
        }
    }
}
