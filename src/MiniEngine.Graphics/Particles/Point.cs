using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;

namespace MiniEngine.Graphics.Particles
{
    [Service]
    public sealed class Point : IDisposable
    {
        private readonly IndexBuffer Indices;
        private readonly VertexBuffer Vertices;

        public Point(GraphicsDevice device)
        {
            var vertex = new PointVertex(Vector3.Zero);

            this.Vertices = new VertexBuffer(device, PointVertex.Declaration, 1, BufferUsage.WriteOnly);
            this.Vertices.SetData(new[] { vertex });

            this.Indices = new IndexBuffer(device, IndexElementSize.SixteenBits, 1, BufferUsage.WriteOnly);
            this.Indices.SetData(new short[] { 0 });
        }

        public void RenderInstanced(GraphicsDevice device, VertexBuffer instances, int count)
        {
            device.SetVertexBuffers(new VertexBufferBinding(this.Vertices), new VertexBufferBinding(instances, 0, 1));
            device.Indices = this.Indices;

            device.DrawInstancedPrimitives(PrimitiveType.PointList, 0, 0, 1, count);
        }

        public void Dispose()
        {
            this.Vertices.Dispose();
            this.Indices.Dispose();
        }
    }
}
