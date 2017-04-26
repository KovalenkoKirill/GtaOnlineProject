using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient.Logger
{
    class EmptyLogger : ILogger
    {
        public void Exception(string message, Exception exception)
        {

        }

        public void Info(string message)
        {

        }

        public void Trace(string message)
        {

        }
    }
}
