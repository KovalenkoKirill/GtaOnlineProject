using DataContact;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer
{
    [Magic]
    public class Client : INotifyPropertyChanged
    {
        public NetConnection NetConnection { get; internal set; }

        public string Session
        {
            get
            {
                if (_session == null) throw new NullSessionException(this, "Client session don't be null");
                return Convert.ToBase64String(_session);
            }
        }

        public ObservableCollection<Client> NearPlayers = new ObservableCollection<Client>();

        public float Latency { get; set; }

        public DateTime lastUpdateTime { get; set; }

        public ServerPlayer Player { get; internal set; }

        public PedInfo Ped
        {
            get
            {
                return Player.player.PedInfo;
            }
            set
            {
                Player.player.PedInfo = value;
            }
        }

        public byte[] _session { get; private set; }

        public Client(NetConnection connection, ServerPlayer player)
        {
            this.NetConnection = connection;
            this.Player = player;
            this._session = Utility.GetSessionToken();
        }

        public DateTime SessionCreatingTime
        {
            get
            {
                if (string.IsNullOrEmpty(Session)) throw new NullSessionException(this, "Client session don't be null");
                return Utility.GetSessionDateTime(this._session);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public NetSendResult Send<T>(T obj,NetDeliveryMethod method = NetDeliveryMethod.Unreliable,int chanel = 0)
        {
            StandardPackage<T> package = new StandardPackage<T>(obj, this._session);
            NetOutgoingMessage responseMessage = Server.Instanse.NetServer.CreateMessage();
            byte[] responseBytes = package.Serialize();
            responseMessage.Data = responseBytes;
            responseMessage.LengthBytes = responseBytes.Length;
            return NetConnection.SendMessage(responseMessage, method, chanel);
        }

        public override int GetHashCode()
        {
            return Session.GetHashCode();
        }
    }
}
