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

        [ProtoMember(2)]
        public byte[] hash { get; set; }

        [ProtoMember(3, DynamicType = true)]
        public T data { get; set; }

        public bool HasSession
        {
            get
            {
                return hash != null;
            }
        }

        public string Session
        {
            get
            {
                return Convert.ToBase64String(hash);
            }
        }

        public StandardPackage() { }

        public StandardPackage(T obj,byte[] hash)
        {
            PacketType type = typeof(T).GetAttributeValue<DataTypeAttribute, PacketType>(x => x.Type);
            if(type == PacketType.unknown)
            {
                throw new NullDataTypeException(typeof(T));
            }
            this.data = obj;
            this.hash = hash;
            if(this.hash == null)
            {
                this.hash = new byte[0];
            }
            this.type = type;
        }

        public StandardPackage(T obj, byte[] hash, PacketType type)
        {
            this.data = obj;
            this.hash = hash;
            this.type = type;
            if (this.hash == null)
            {
                this.hash = new byte[0];
            }
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
                stream.Seek(0, SeekOrigin.Begin);
                try
                {
                    StandardPackage<T> result;
                    result = Serializer.Deserialize<StandardPackage<T>>(stream);
                    return result;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
    }
}
