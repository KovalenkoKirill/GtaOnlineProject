using DataContact;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer
{
    [Magic]
    public class ServerPlayer : INotifyPropertyChanged
    {
        public float Latency { get; set; }

        public Player player { get; set; }

        public string Login { get; set; }

        public ServerPlayer(string name)
        {
            player = new Player();
            player.PlayerId = Guid.NewGuid();
        }

        public ServerPlayer()
        {

        }

        public static implicit operator Player(ServerPlayer player)
        {
            return player.player;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); // некоторые из нас здесь используют Dispatcher, для безопасного взаимодействия с UI thread
        }

        private uint GetRandom()
        {
            byte[] buffer = new byte[4];
            new Random().NextBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }
    }
}
