using DataContact;
using GtaServer.DataContact;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{

    [ProtoContract]
    public class StandardPackage<T>
    {
        [ProtoMember(1)]
        public PacketType type { get; set; }

        [ProtoMember(2, DataFormat = DataFormat.FixedSize)]
        public byte[] hash { get; set; }

        [ProtoMember(3, DynamicType = true)]
        public T data { get; set; }

        public StandardPackage(T obj,byte[] hash)
        {
            PacketType type = typeof(T).GetAttributeValue<DataTypeAttribute, PacketType>(x => x.Type);
            if(type == PacketType.unknown)
            {
                throw new NullDataTypeException(typeof(T));
            }
            this.data = obj;
            this.hash = hash;
            this.type = type;
        }

        public StandardPackage(T obj, byte[] hash, PacketType type)
        {
            this.data = obj;
            this.hash = hash;
            this.type = type;
        }

        public byte [] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, this);
                return stream.ToArray();
            }
        }

        public static StandardPackage<T> GetStandardPackage(byte [] _input)
        {
            using (var stream = new MemoryStream(_input))
            {
                return Serializer.Deserialize<StandardPackage<T>>(stream);
            }
        }
    }
}
