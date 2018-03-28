using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MiniEngine.Mathematics
{
    public static class Vector4Extensions
    {
        public static Vector3 ToVector3(this Vector4 value)
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
    }
}
