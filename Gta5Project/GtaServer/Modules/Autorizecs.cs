using DataContact;
using GtaServer.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer.Modules
{
    public class Authorization : IAuthorization
    {
        public ServerPlayer Autorize(string login, string pass)
        {
            if(Server.Instanse.configuration.devMode)
            {
                if (login.ToLower().Contains("dev"))
                {
                    ServerPlayer player = Server.Instanse.DataManager.GetServerPlayer(login);
                    if(player == null)
                    {
                        player = Server.Instanse.DataManager.GetDefault("dev");
                    }
                    player.player.PlayerId = Guid.NewGuid();
                    player.player.PedInfo.PedId = (uint)new Random().Next();
                    player.Login = login;
                    return player;
                }
            }
            else
            {
                ///Тут боевой код с лоигкой
            }
            return null;
        }
    }
}
