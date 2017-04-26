#if TEST
#else
using GtaClient;
#endif
using DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if TEST
using MainClient = TestGtaProject.FakeClient;
#endif

namespace TestGtaProject
{
    public class PlayerSynchronization:IDisposable
    {

        public Thread SyncThread;

        MainClient client;

        bool IsWork = true;

        public PlayerSynchronization(MainClient client)
        {
            this.client = client;
            SyncThread = new Thread(SyncTask);
            SyncThread.Start();
        }

        public void Dispose()
        {
            try
            {
                IsWork = false;
                this.SyncThread.Abort();
            }
            catch { }
        }

        void SyncTask()
        {

        }
    }
}
