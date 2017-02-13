using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{
    public class NullDataTypeException:Exception
    {
        public new string Message { get; private set; }

        public NullDataTypeException(Type t)
        {
            string message = $"{t.Name} do not contains attribute DataType";
            this.Message = message;
        }
    }
}
