using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace MiniEngine.Net.Serializers
{
    public static class Vector3Serializer
    {
        public static void Write(NetDataWriter writer, Vector3 value)
        {
            writer.Put(value.X);
            writer.Put(value.Y);
            writer.Put(value.Z);
        }

        public static Vector3 Read(NetDataReader reader)
            => new Vector3(reader.GetFloat(), reader.GetFloat(), reader.GetFloat());
    }
}
