using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives.VertexTypes;

namespace MiniEngine.Primitives
{
    /// <summary>
    /// A Quad that can be wrapped around 3D objects covering them entirely in screen coordinates
    /// </summary>
    public sealed class WrappableQuad
    {
        private readonly GraphicsDevice Device;
        private readonly GBufferVertex[] Vertices;
        private readonly short[] Indices;
        private readonly short[] LineIndices;
        
        private readonly Vector3[] Corners;

        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        public WrappableQuad(GraphicsDevice device)
        {
            this.Device = device;

            this.Vertices = new[]
            {
                new GBufferVertex(
                    new Vector3(-1, -1, 0),
                    new Vector2(0, 1)),
                new GBufferVertex(
                    new Vector3(-1, 1, 0),
                    new Vector2(0, 0)),                                
                new GBufferVertex(
                    new Vector3(1, -1, 0),
                    new Vector2(1, 1)),
                    new GBufferVertex(
                    new Vector3(1, 1, 0),
                    new Vector2(1, 0)),
            };

            this.Indices = new short[] { 0, 1, 2, 3};
            this.LineIndices = new short[] { 0, 1, 3, 2, 0 };
            this.Corners = new Vector3[BoundingBox.CornerCount];
        }

        public void WrapOnScreen(BoundingBox bounds, PerspectiveCamera camera)
        {
            bounds.GetCorners(this.Corners);
            this.WrapOnScreen(this.Corners, camera);
        }

        public void WrapOnScreen(BoundingFrustum frustum, PerspectiveCamera camera)
        {
            frustum.GetCorners(this.Corners);
            this.WrapOnScreen(this.Corners, camera);
        }

        public void Reset()
        {
            this.minX = -1.0f;
            this.maxX = 1.0f;

            this.minY = -1.0f;
            this.maxY = 1.0f;

            this.Vertices[0].Position = new Vector4(this.minX, this.minY, 0, 1);
            this.Vertices[0].TextureCoordinate = ProjectionMath.ToUv(this.minX, this.minY);

            this.Vertices[1].Position = new Vector4(this.minX, this.maxY, 0, 1);
            this.Vertices[1].TextureCoordinate = ProjectionMath.ToUv(this.minX, this.maxY);

            this.Vertices[2].Position = new Vector4(this.maxX, this.minY, 0, 1);
            this.Vertices[2].TextureCoordinate = ProjectionMath.ToUv(this.maxX, this.minY);

            this.Vertices[3].Position = new Vector4(this.maxX, this.maxY, 0, 1);
            this.Vertices[3].TextureCoordinate = ProjectionMath.ToUv(this.maxX, this.maxY);
        }

        /// <summary>
        /// Computes the projected coordinates of the corners and then wraps
        /// the quad around them on screen. Scales the UV coordinates accordingly
        /// </summary>
        public void WrapOnScreen(Vector3[] corners, PerspectiveCamera camera)
        {
            for (var i = 0; i < corners.Length; i++)
            {
                var corner = corners[i];

                // TODO: can't we compute the corner as if it is on the camera's near plane (max size)
                // instead of doing this???
                // If one of the corners is behind the camera the computed 2D space 
                // is wrong so cover the entire screen.
                if (IsBehindCamera(corner, camera))
                {
                    this.Reset();
                    return;
                }
            }

            this.minX = float.MaxValue;
            this.maxX = float.MinValue;

            this.minY = float.MaxValue;
            this.maxY = float.MinValue;

            for (var i = 0; i < corners.Length; i++)
            {
                var corner = corners[i];
                
                var projectedCorner = ProjectionMath.WorldToView(corner, camera.Position, camera.Forward, camera.ViewProjection);

                this.minX = Math.Min(this.minX, projectedCorner.X);
                this.maxX = Math.Max(this.maxX, projectedCorner.X);

                this.minY = Math.Min(this.minY, projectedCorner.Y);
                this.maxY = Math.Max(this.maxY, projectedCorner.Y);
            }

            this.Vertices[0].Position = new Vector4(this.minX, this.minY, 0, 1);
            this.Vertices[0].TextureCoordinate = ProjectionMath.ToUv(this.minX, this.minY);

            this.Vertices[1].Position = new Vector4(this.minX, this.maxY, 0, 1);
            this.Vertices[1].TextureCoordinate = ProjectionMath.ToUv(this.minX, this.maxY);

            this.Vertices[2].Position = new Vector4(this.maxX, this.minY, 0, 1);
            this.Vertices[2].TextureCoordinate = ProjectionMath.ToUv(this.maxX, this.minY);

            this.Vertices[3].Position = new Vector4(this.maxX, this.maxY, 0, 1);
            this.Vertices[3].TextureCoordinate = ProjectionMath.ToUv(this.maxX, this.maxY);
        }

        private static bool IsBehindCamera(Vector3 corner, PerspectiveCamera camera)
        {
            var cornerDirection = Vector3.Normalize(corner - camera.Position );
            var dot = Vector3.Dot(camera.Forward, cornerDirection);
           
            return dot < 0;
        }
       
        public void Render() 
            => this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, this.Vertices, 0, 4, this.Indices, 0, 2);

        
        public void RenderOutline()
            => this.Device.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, this.Vertices, 0, 4, this.LineIndices, 0, 4);
    }
}
