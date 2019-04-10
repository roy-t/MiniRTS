using Microsoft.Xna.Framework;
using System;

namespace MiniEngine.Primitives.Bounds
{
    public readonly struct BoundingRectangle
    {
        public BoundingRectangle(float minX, float maxX, float minY, float maxY)
        {
            this.MinX = minX;
            this.MaxX = maxX;
            this.MinY = minY;
            this.MaxY = maxY;
        }

        public float MinX { get; }
        public float MaxX { get; }
        public float MinY { get; }
        public float MaxY { get; }

        public bool Intersects(BoundingRectangle rectangle)
        {
            // Intersects
            if (this.MaxX >= rectangle.MinX && this.MinX <= rectangle.MaxX)
            {
                if (this.MaxY >= rectangle.MinY && this.MinY <= rectangle.MaxY)
                {
                    return true;
                }
            }

            // Contains
            if (this.MinX <= rectangle.MinX && this.MaxX >= rectangle.MaxX)
            {
                if (this.MinY <= rectangle.MinY && this.MaxY >= rectangle.MaxY)
                {
                    return true;
                }
            }

            return false;
        }

        public Vector2[] GetCorners()
        {
            return new[]
            {
                new Vector2(this.MinX, this.MaxY),
                new Vector2(this.MaxX, this.MaxY),
                new Vector2(this.MaxX, this.MinY),
                new Vector2(this.MinX, this.MinY)
            };
        }

        public static BoundingRectangle CreateMerged(BoundingRectangle rectA, BoundingRectangle rectB)
        {
            var minX = Math.Min(rectA.MinX, rectB.MinX);
            var maxX = Math.Max(rectA.MaxX, rectB.MaxX);

            var minY = Math.Min(rectA.MinY, rectB.MinY);
            var maxY = Math.Max(rectA.MaxY, rectB.MaxY);

            return new BoundingRectangle(minX, maxX, minY, maxY);
        }

        public static BoundingRectangle CreateFromProjectedBoundingBox(BoundingBox box, Matrix matrix)
        {
            var minX = float.MaxValue;
            var maxX = float.MinValue;

            var minY = float.MaxValue;
            var maxY = float.MinValue;

            var corners = box.GetCorners();

            for (var i = 0; i < corners.Length; i++)
            {
                var corner = corners[i];
                var projectedCorner = WorldToView(corner, matrix);

                minX = Math.Min(minX, projectedCorner.X);
                maxX = Math.Max(maxX, projectedCorner.X);

                minY = Math.Min(minY, projectedCorner.Y);
                maxY = Math.Max(maxY, projectedCorner.Y);
            }

            return new BoundingRectangle(minX, maxX, minY, maxY);
        }

        private static Vector2 WorldToView(Vector3 worldPosition, Matrix viewProjection)
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