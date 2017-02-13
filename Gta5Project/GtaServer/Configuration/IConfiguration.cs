using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer.Configuration
{
    public interface IConfiguration: INotifyPropertyChanged
    {
        int serverPort { get; }

        int maxPlayers { get; }

        ILogger logger { get; }

        int maxThread { get; }

        bool devMode { get; }
    }
}
