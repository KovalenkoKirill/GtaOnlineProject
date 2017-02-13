using GtaServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace TestGtaProject.forServer
{
    public class TestConfiguration : IConfiguration
    {
        internal bool _devMode = true;

        internal ILogger _logger;

        internal int _maxPlayer = 1000;

        internal int _serverPort = 5999;

        internal int _maxThread = 1000;


        public TestConfiguration()
        {
            _logger = new TestIlogger();
        }


        public bool devMode
        {
            get
            {
                return _devMode;
            }
            
        }

        public ILogger logger
        {
            get
            {
                return _logger;
            }
        }

        public int maxPlayers
        {
            get
            {
                return _maxPlayer;
            }
        }

        public int maxThread
        {
            get
            {
                return _maxThread;
            }
        }

        public int serverPort
        {
            get
            {
                return _serverPort;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); 
        }
    }
}
