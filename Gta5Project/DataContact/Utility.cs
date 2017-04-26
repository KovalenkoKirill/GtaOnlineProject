using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContact
{
    public static class Utility
    {
        public static float Distanse(LVector3 vector1, LVector3 vector2)
        {
            return (float)Math.Sqrt(Math.Pow((vector1.X - vector2.X), 2) +
                Math.Pow((vector1.Y - vector2.Y), 2)
                + Math.Pow((vector1.Z - vector2.Z), 2));
        }

        public static byte[] GetSessionToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return time.Concat(key).ToArray();
        }
        public static byte[] GetSessionToken(DateTime sessionTime)
        {
            byte[] time = BitConverter.GetBytes(sessionTime.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return time.Concat(key).ToArray();
        }
        public static DateTime GetSessionDateTime(byte [] session)
        {
            DateTime when = DateTime.FromBinary(BitConverter.ToInt64(session, 0));
            return when;
        }
    }
}
