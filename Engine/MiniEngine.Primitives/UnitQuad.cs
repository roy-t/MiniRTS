using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.VertexTypes;

namespace MiniEngine.Primitives
{
    /// <summary>
    /// A Quad with 2x2 dimensions centered around {0, 0, 0}. Covers the entire screen when rendered
    /// with the identity WVP matrix. Has adjustable UV coordinates
    /// </summary>
    public sealed class UnitQuad
    {
        private readonly GraphicsDevice Device;
        private readonly short[] Indices;
        private readonly GBufferVertex[] Vertices;

        public UnitQuad(GraphicsDevice device)
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

            this.Indices = new short[] { 0, 1, 2, 3 };
        }

        public void SetTextureCoordinates(Vector2 minUv, Vector2 maxUv)
        {
            this.Vertices[0] = new GBufferVertex(this.Vertices[0].Position, minUv.X, minUv.Y);
            this.Vertices[1] = new GBufferVertex(this.Vertices[1].Position, minUv.X, maxUv.Y);
            this.Vertices[2] = new GBufferVertex(this.Vertices[2].Position, maxUv.X, minUv.Y); ;
            this.Vertices[3] = new GBufferVertex(this.Vertices[3].Position, maxUv.X, maxUv.Y); ;
        }

        public void Render()
             => this.Device.DrawUserIndexedPrimitives(PrimitiveType.TriangleStrip, this.Vertices, 0, 4, this.Indices, 0, 2);
    }
}