using GTA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GtaClient
{
    public class GameTaskFactory : Script, IDisposable
    {
        public static GameTaskFactory Factory;

        ManualResetEvent reserEvent = new ManualResetEvent(false);

        List<Action> actions = new List<Action>();

        public new int Interval
        {
            get
            {
                return 20;
            }
        }

        public GameTaskFactory()
        {

            Factory = this;
            this.Tick += GameTaskFactory_Tick;
        }

        bool isInit = false;

        private void GameTaskFactory_Tick(object sender, EventArgs e)
        {
            isInit = true;
            invokeAll();
        }

        object sync = new object();

        public void invokeAll()
        {
            lock (sync)
            {
                foreach (var action in actions)
                {
                    try
                    {
                        action.Invoke();
                    }
                    catch (Exception e)
                    {
                        MainClient.Instance.Logger.Exception("invokeAll ex", e);
                    }
                }
                actions.Clear();
            }
            reserEvent.Set();
            reserEvent.Reset();
        }

        public void Invoke(Action action)
        {
            if (isInit)
            {
                lock (sync)
                {
                    actions.Add(action);
                }
                reserEvent.WaitOne(2000);
            }
            else
            {
               // action.Invoke();
            }
        }

        public void Dispose()
        {
            reserEvent.Dispose();
        }
    }
}
