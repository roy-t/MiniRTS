using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.Primitives
{
    public static class ProjectionMath
    {
        /// <summary>
        /// Converts screen-space {X, Y} coordinates to texture coordinates
        /// So from {[-1..1], [-1..1]} to {[0..1], [1..0]}
        /// </summary>
        public static Vector2 ToUv(float x, float y)
            => new Vector2((x * 0.5f) + 0.5f, (y * -0.5f) + 0.5f);

        /// <summary>
        /// Converts a world position to a position on screen
        /// </summary>
        public static Vector2 WorldToView(Vector3 worldPosition, Matrix viewProjection)
        {
            var x = (worldPosition.X * viewProjection.M11) + (worldPosition.Y * viewProjection.M21)
                                                           + (worldPosition.Z * viewProjection.M31)
                                                           + viewProjection.M41;

            var y = (worldPosition.X * viewProjection.M12) + (worldPosition.Y * viewProjection.M22)
                                                           + (worldPosition.Z * viewProjection.M32)
                                                           + viewProjection.M42;

            var w = (worldPosition.X * viewProjection.M14) + (worldPosition.Y * viewProjection.M24)
                                                           + (worldPosition.Z * viewProjection.M34)
                                                           + viewProjection.M44;

            // w is negative when the camera has zoomed beyond the world position in which case
            // it needs to be negated to be able to compute the correct screen position
            w = Math.Abs(w);

            x = x / w;
            y = y / w;

            return new Vector2(x, y);
        }        
    }
}
