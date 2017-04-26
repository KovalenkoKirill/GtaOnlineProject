using DataContact;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GtaClient
{
    public static class BaseTypeExtention
    {

        public static LVector3 ToLVector(this Vector3 vector)
        {
            return new LVector3(vector.X, vector.Y, vector.Z);
        }
        public static Vector3 ToVector(this LVector3 vector)
        {
            return new Vector3(vector.X, vector.Y, vector.Z);
        }

        public static LQuaternion ToLQuaternion(this Quaternion quaternion)
        {
            return new LQuaternion()
            {
                X = quaternion.X,
                Z = quaternion.Z,
                W = quaternion.W,
                Y = quaternion.Y
            };
        }
        public static Quaternion ToQuaternion(this LQuaternion quaternion)
        {
            return new Quaternion()
            {
                X = quaternion.X,
                Z = quaternion.Z,
                W = quaternion.W,
                Y = quaternion.Y
            };
        }
    }
}
