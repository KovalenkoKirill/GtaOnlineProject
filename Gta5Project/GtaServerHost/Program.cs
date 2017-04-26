using GtaServer;
using GtaServerHost.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServerHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerConfuguration configuration = GetConfiguration();
            Server server = new Server(configuration);
            server.Start();
        }

        static ServerConfuguration GetConfiguration()
        {
            using (StreamReader reader = new StreamReader("Configuration.json"))
            {
                string confsString = reader.ReadToEnd();
                ServerConfuguration configuraton = JsonConvert.DeserializeObject<ServerConfuguration>(confsString);
                return configuraton;
            }
        }
    }
}
