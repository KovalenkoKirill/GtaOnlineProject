using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient
{
    public class ClientConfiguration
    {
        public string ServerAdress { get; set; }

        public int ServerPort { get; set; }

        public ClientConfiguration()
        {
        }
    }
}
