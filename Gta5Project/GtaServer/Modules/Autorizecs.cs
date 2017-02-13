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
        public Player Autorize(string login, string pass)
        {
            if(Server.Instanse.configuration.devMode)
            {
                if(login.ToLower() == "dev" && pass.ToLower() == "dev")
                return new Player()
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
