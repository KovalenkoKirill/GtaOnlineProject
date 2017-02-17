using GtaClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGtaProject
{
    public class PlayerSynchronization
    {
#if TEST
        public PlayerSynchronization(FakeClient client)
#else
        public PlayerSynchronization(MainClient client)
#endif
        {

        }
    }
}
