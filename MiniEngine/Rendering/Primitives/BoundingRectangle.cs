using System;
using Microsoft.Xna.Framework;
using MiniEngine.Utilities.Extensions;

namespace MiniEngine.Rendering.Primitives
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

            foreach (var corner in corners)
            {
                var projectedCorner = Vector4.Transform(corner, matrix).ScaleToVector3();
                minX = Math.Min(minX, projectedCorner.X);                
                maxX = Math.Max(maxX, projectedCorner.X);

                minY = Math.Min(minY, projectedCorner.Y);
                maxY = Math.Max(maxY, projectedCorner.Y);
            }

            return new BoundingRectangle(minX, maxX, minY, maxY);
        }
    }
}
