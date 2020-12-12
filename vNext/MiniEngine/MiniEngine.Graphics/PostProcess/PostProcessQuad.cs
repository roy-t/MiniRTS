using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Graphics.PostProcess
{
    // TODO: totally broken!

    [Service]
    public sealed class PostProcessQuad
    {
        private readonly Vector3[] Corners;

        private readonly PostProcessVertex[] Vertices;
        private readonly short[] Indices;

        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        public PostProcessQuad()
        {
            this.Corners = new Vector3[BoundingBox.CornerCount];
            this.Vertices = new PostProcessVertex[4];
            this.Indices = new short[] { 0, 1, 2, 2, 3, 0 };
        }

        public void RenderOutline(GraphicsDevice device, BoundingFrustum frustum, ICamera camera)
        {
            throw new Exception("BROKEN");
            frustum.GetCorners(this.Corners);
            this.Wrap(this.Corners, camera);
            device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.Vertices, 0, 4, this.Indices, 0, 2);
        }

        private void Wrap(Vector3[] corners, ICamera camera)
        {
            this.minX = float.MaxValue;
            this.maxX = float.MinValue;

            this.minY = float.MaxValue;
            this.maxY = float.MinValue;

            for (var i = 0; i < BoundingBox.CornerCount; i++)
            {
                var corner = corners[i];

                var projectedCorner = WorldToView(corner, camera.ViewProjection);

                if (IsBehindCamera(corner, camera))
                {
                    projectedCorner.X = -projectedCorner.X;
                    projectedCorner.Y = -projectedCorner.Y;
                }

                this.minX = Math.Min(this.minX, projectedCorner.X);
                this.maxX = Math.Max(this.maxX, projectedCorner.X);

                this.minY = Math.Min(this.minY, projectedCorner.Y);
                this.maxY = Math.Max(this.maxY, projectedCorner.Y);
            }

            this.Vertices[0].Position = new Vector3(this.minX, this.minY, 0);
            //this.Vertices[0].Te= new Vector3(this.minX, this.minY, 0);
            this.Vertices[1].Position = new Vector3(this.minX, this.maxY, 0);
            this.Vertices[2].Position = new Vector3(this.maxX, this.minY, 0);
            this.Vertices[3].Position = new Vector3(this.maxX, this.maxY, 0);
        }

        private static bool IsBehindCamera(Vector3 point, ICamera camera)
        {
            var cornerDirection = Vector3.Normalize(point - camera.Position);
            var dot = Vector3.Dot(camera.Forward, cornerDirection);

            return dot < 0;
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

            if (!WithinEpsilon(w, 1.0f))
            {
                x = x / w;
                y = y / w;
            }

            return new Vector2(x, y);
        }

        private static bool WithinEpsilon(float a, float b)
        {
            float num = a - b;
            if (-1.401298E-45f <= num)
            {
                return num <= 1.401298E-45f;
            }
            return false;
        }
    }
}
