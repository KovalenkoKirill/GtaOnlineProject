using Lidgren.Network;
using System;
using System.Collections.Generic;
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

        public float Latency { get; set; }

        public Player Player { get; internal set; }

        public byte[] _session { get; private set; }

        public Client(NetConnection connection,Player player)
        {
            this.NetConnection = connection;
            this.Player = player;
        }

        public DateTime SessionCreatingTime
        {
            get
            {
                if (string.IsNullOrEmpty(Session)) throw new NullSessionException(this, "Client session don't be null");
                DateTime when = DateTime.FromBinary(BitConverter.ToInt64(this._session, 0));
                return when;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void GetSessionToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            this._session = time.Concat(key).ToArray();
        }

        public override int GetHashCode()
        {
            return Session.GetHashCode();
        }
    }
}
