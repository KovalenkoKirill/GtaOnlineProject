using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataContact;

namespace GtaServer
{
    [Handle(DataContact.PacketType.pedInfo)]
    class UpdatePlayerPosotion : IHandler
    {
        public void Dispose()
        {
           
        }


        public Task HandlePackage<T>(StandardPackage<T> package, Client client)
        {
            try
            {
                PedInfo info = package.data as PedInfo;
                client.Ped = info;
            }catch(Exception ex)
            {
                Server.Instanse.Logger.Exception("", ex);
            }
            return null;
        }
    }
}
