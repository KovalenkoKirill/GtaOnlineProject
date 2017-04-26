using DataContact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient.Core
{
    public interface IHandler : IDisposable
    {
        Task HandlePackage<T>(StandardPackage<T> package);
    }
}
