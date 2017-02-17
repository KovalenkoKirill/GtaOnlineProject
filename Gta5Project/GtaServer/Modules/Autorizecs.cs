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
                if(login.ToLower() == "dev")
                return new ServerPlayer()
                {
                    DisplayName = "Player1",
                    Health = 100
                };
            }
            else
            {
                ///Тут боевой код с лоигкой
            }
            return null;
        }
    }
}
