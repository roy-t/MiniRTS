using Microsoft.Xna.Framework;
using System;

namespace MiniEngine.Utilities.Extensions
{
    public static class VectorExtensions
    {
        public static Vector3 ScaleToVector3(this Vector4 value)
        {
            return new Vector3(value.X, value.Y, value.Z) / value.W;
        }

        public static Vector4 Round(this Vector4 value)
        {
            return new Vector4(
                (float)Math.Round(value.X),
                (float)Math.Round(value.Y),
                (float)Math.Round(value.Z),
                (float)Math.Round(value.W));
        }

        public static Vector3 Round(this Vector3 value)
        {
            return new Vector3(
                (float) Math.Round(value.X),
                (float) Math.Round(value.Y),
                (float) Math.Round(value.Z));
        }
    }
}
