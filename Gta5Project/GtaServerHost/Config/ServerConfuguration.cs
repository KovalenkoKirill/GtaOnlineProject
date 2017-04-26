using GtaServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GtaServerHost.Config
{
    public class ServerConfuguration : IConfiguration
    {
        public bool devMode { get; set; }

        public ILogger logger { get; set; }

        public int maxPlayers { get; set; }

        public int maxThread { get; set; }

        public int serverPort { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ServerConfuguration()
        {
            logger = new Logger();
        }
    }
}
