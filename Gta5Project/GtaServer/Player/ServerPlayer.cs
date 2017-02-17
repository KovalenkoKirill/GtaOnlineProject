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

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public Vector3 Position { get; set; }

        public int Health { get; set; }

        public int VehicleHealth { get; set; }

        public PlayerFlags flags { get; set; }

        public static implicit operator Player(ServerPlayer player)
        {
            return new Player()
            {
                DisplayName = player.DisplayName,
                flags = player.flags,
                Health = player.Health,
                Name = player.Name,
                Position = player.Position,
                VehicleHealth = player.VehicleHealth
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); // некоторые из нас здесь используют Dispatcher, для безопасного взаимодействия с UI thread
        }
    }
}
