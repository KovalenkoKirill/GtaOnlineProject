using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataContact;

namespace GtaServer
{
    [Handle(DataContact.PacketType.playerInfo)]
    class UpdatePlayerPosotion : IHandler
    {
        public void Dispose()
        {
           
        }


        public Task HandlePackage<T>(StandardPackage<T> package, Client client)
        {
            PlayerInfo info = package.data as PlayerInfo;
            client.Player.flags = info.flags;
            client.Player.Position = info.Position;
            client.Player.VehicleHealth = info.VehicleHealth;
            client.Player.Health = info.Health;
            return null;
        }
    }
}
