using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaServer
{
    public class NullSessionException:Exception
    {
        public Client client { get; private set; }

        public NullSessionException(Client client,string message):base(message)
        {
            this.client = client;
        }
    }
}
