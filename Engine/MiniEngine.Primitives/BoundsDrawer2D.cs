using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Primitives
{
    /// <summary>
    /// Draws 2D quad to highlight the bounds of 3D objects on-screen
    /// </summary>
    public sealed class BoundsDrawer2D
    {
        private readonly GraphicsDevice Device;
        private readonly Vector3[] Corners;

        private readonly VertexPosition[] Vertices;
        private readonly short[] Indices;

        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        public BoundsDrawer2D(GraphicsDevice device)
        {
            this.Device = device;
            this.Corners = new Vector3[BoundingBox.CornerCount];
            this.Vertices = new VertexPosition[4];
            this.Indices = new short[] { 0, 1, 3, 2, 0 };
        }


        public void RenderOutline(BoundingSphere boundingSphere, PerspectiveCamera camera)
            => this.RenderOutline(BoundingBox.CreateFromSphere(boundingSphere), camera);

        public void RenderOutline(Vector3[] corners, PerspectiveCamera camera)
        {
            corners.CopyTo(this.Corners, 0);
            this.Wrap(this.Corners, camera);
            this.RenderOutline();
        }

        public void RenderOutline(BoundingBox bounds, PerspectiveCamera camera)
        {
            bounds.GetCorners(this.Corners);
            this.Wrap(this.Corners, camera);
            this.RenderOutline();
        }

        public void RenderOutline(BoundingFrustum frustum, PerspectiveCamera camera)
        {
            frustum.GetCorners(this.Corners);
            this.Wrap(this.Corners, camera);
            this.RenderOutline();
        }

        private void Wrap(Vector3[] corners, PerspectiveCamera camera)
        {
            this.minX = float.MaxValue;
            this.maxX = float.MinValue;

            this.minY = float.MaxValue;
            this.maxY = float.MinValue;

            for (var i = 0; i < BoundingBox.CornerCount; i++)
            {
                var corner = corners[i];

                var projectedCorner = ProjectionMath.WorldToView(corner, camera.ViewProjection);

                if (ProjectionMath.IsBehindCamera(corner, camera))
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
            this.Vertices[1].Position = new Vector3(this.minX, this.maxY, 0);
            this.Vertices[2].Position = new Vector3(this.maxX, this.minY, 0);
            this.Vertices[3].Position = new Vector3(this.maxX, this.maxY, 0);
        }

        private void RenderOutline()
            => this.Device.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, this.Vertices, 0, 4, this.Indices, 0, 4);
    }
}
