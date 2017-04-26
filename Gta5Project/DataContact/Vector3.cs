using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{
    [ProtoContract]
    public class LVector3
    {
        public LVector3()
        {
            X = 0f;
            Y = 0f;
            Z = 0f;
        }

        public LVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [ProtoMember(1)]
        public float X { get; set; }
        [ProtoMember(2)]
        public float Y { get; set; }
        [ProtoMember(3)]
        public float Z { get; set; }
    }
    [ProtoContract]
    public class LQuaternion
    {
        [ProtoMember(1)]
        public float X { get; set; }
        [ProtoMember(2)]
        public float Y { get; set; }
        [ProtoMember(3)]
        public float Z { get; set; }
        [ProtoMember(4)]
        public float W { get; set; }

    }
}
