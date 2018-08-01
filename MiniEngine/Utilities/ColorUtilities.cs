using Microsoft.Xna.Framework;
using System;

namespace MiniEngine.Utilities
{
    public static class ColorUtilities
    {
        private static readonly Random Random = new Random();

        public static Color PickRandomColor()
        {
            var r = Random.NextDouble();
            var g = Random.NextDouble();
            var b = Random.NextDouble();

            var vector = new Vector3((float) r, (float) g, (float) b);
            vector.Normalize();

            return new Color(vector);
        }

    }
}
