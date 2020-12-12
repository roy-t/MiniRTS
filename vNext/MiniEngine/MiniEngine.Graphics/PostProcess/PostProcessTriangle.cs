using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Graphics.PostProcess
{
    [Service]
    public sealed class PostProcessTriangle : IDisposable
    {
        private readonly IndexBuffer Indices;
        private readonly VertexBuffer Vertices;

        public PostProcessTriangle(GraphicsDevice device)
        {
            var vertices = new[]
            {
                new PostProcessVertex(
                    new Vector3(3, -1, 0),
                    new Vector2(2, 1)),
                new PostProcessVertex(
                    new Vector3(-1, -1, 0),
                    new Vector2(0, 1)),
                new PostProcessVertex(
                    new Vector3(-1, 3, 0),
                    new Vector2(0, -1))
            };

            var indices = new short[]
            {
                0,
                1,
                2
            };

            this.Vertices = new VertexBuffer(device, PostProcessVertex.Declaration, vertices.Length, BufferUsage.None);
            this.Vertices.SetData(vertices);

            this.Indices = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            this.Indices.SetData(indices);
        }

        public void Render(GraphicsDevice device)
        {
            device.SetVertexBuffer(this.Vertices);
            device.Indices = this.Indices;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 1);
        }

        public void Dispose()
        {
            this.Vertices.Dispose();
            this.Indices.Dispose();
        }
    }
}
