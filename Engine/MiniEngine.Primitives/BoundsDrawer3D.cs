using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Primitives
{
    /// <summary>
    /// Draws 3D bounding primitives to highlight objects on screen
    /// </summary>
    public sealed class BoundsDrawer3D
    {
        private const int CornerCount = 8;
        private const int TriangleCount = 12;

        private readonly GraphicsDevice Device;
        private readonly Vector3[] Corners;

        private readonly VertexPosition[] Vertices;
        private readonly short[] Indices;
        private readonly short[] LineIndices;

        public BoundsDrawer3D(GraphicsDevice device)
        {
            this.Device = device;
            this.Corners = new Vector3[CornerCount];
            this.Vertices = new VertexPosition[CornerCount];

            const int nearTopLeft = 0;
            const int nearTopRight = 1;
            const int nearBottomRight = 2;
            const int nearBottomLeft = 3;
            const int farTopLeft = 4;
            const int farTopRight = 5;
            const int farBottomRight = 6;
            const int farBottomLeft = 7;

            this.Indices = new short[]
            {
                // Far plane
                farBottomLeft, farTopLeft, farTopRight,
                farTopRight, farBottomRight, farBottomLeft,

                // Near plane
                nearBottomRight, nearTopRight, nearTopLeft,
                nearTopLeft, nearBottomLeft, nearBottomRight,

                // Left plane
                nearBottomLeft, nearTopLeft, farTopLeft,
                farTopLeft, farBottomLeft, nearBottomLeft,

                // Right plane
                farBottomRight, farTopRight, nearTopRight,
                nearTopRight, nearBottomRight, farBottomRight,

                // Top plane
                nearTopRight, farTopRight, farTopLeft,
                farTopLeft, nearTopLeft, nearTopRight,

                // Bottom plane
                nearBottomLeft, farBottomLeft, farBottomRight,
                farBottomRight, nearBottomRight, nearBottomLeft
            };

            this.LineIndices = new short[]
            {
                // Near plane
                nearTopLeft, nearTopRight, nearTopRight, nearBottomRight, nearBottomRight, nearBottomLeft, nearBottomLeft, nearTopLeft,

                // Far plane
                farTopLeft, farTopRight, farTopRight, farBottomRight, farBottomRight, farBottomLeft, farBottomLeft, farTopLeft,

                // Spines
                nearTopLeft, farTopLeft, nearTopRight, farTopRight, nearBottomRight, farBottomRight, nearBottomLeft, farBottomLeft
            };
        }

        public void Render(BoundingFrustum frustum)
        {
            frustum.GetCorners(this.Corners);
            this.Render();
        }

        public void RenderOutline(BoundingSphere boundingSphere)
            => this.RenderOutline(BoundingBox.CreateFromSphere(boundingSphere));

        public void RenderOutline(Vector3[] corners)
        {
            corners.CopyTo(this.Corners, 0);
            this.RenderOutline();
        }

        public void RenderOutline(BoundingFrustum frustum)
        {
            frustum.GetCorners(this.Corners);
            this.RenderOutline();
        }

        public void Render(BoundingBox bounds)
        {
            bounds.GetCorners(this.Corners);
            this.Render();
        }

        public void RenderOutline(BoundingBox bounds)
        {
            bounds.GetCorners(this.Corners);
            this.RenderOutline();
        }

        private void Render()
        {
            for (var i = 0; i < CornerCount; i++)
            {
                this.Vertices[i].Position = this.Corners[i];
            }

            this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.Vertices, 0, CornerCount,
                this.Indices, 0, TriangleCount);
        }

        private void RenderOutline()
        {
            for (var i = 0; i < CornerCount; i++)
            {
                this.Vertices[i].Position = this.Corners[i];
            }

            this.Device.DrawUserIndexedPrimitives(PrimitiveType.LineList, this.Vertices, 0, this.Vertices.Length,
                this.LineIndices, 0, 12);
        }
    }
}
