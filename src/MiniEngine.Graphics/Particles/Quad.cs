using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;

namespace MiniEngine.Graphics.Particles
{
    [Service]
    public sealed class Quad : IDisposable
    {
        private readonly IndexBuffer Indices;
        private readonly VertexBuffer Vertices;

        public Quad(GraphicsDevice device)
        {
            var vertices = new[]
            {
                new PostProcessVertex(
                    new Vector3(-0.5f, -0.5f, 0),
                    new Vector2(0, 1)),
                new PostProcessVertex(
                    new Vector3(-0.5f, 0.5f, 0),
                    new Vector2(0, 0)),
                new PostProcessVertex(
                    new Vector3(0.5f, 0.5f, 0),
                    new Vector2(1, 0)),
                new PostProcessVertex(
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector2(1, 1))
            };

            var indices = new short[]
            {
                0,
                1,
                2,

                2,
                3,
                0
            };

            this.Vertices = new VertexBuffer(device, PostProcessVertex.Declaration, vertices.Length, BufferUsage.WriteOnly);
            this.Vertices.SetData(vertices);

            this.Indices = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.WriteOnly);
            this.Indices.SetData(indices);
        }

        public void RenderInstanced(GraphicsDevice device, ParticleBuffer particles)
            => this.RenderInstanced(device, particles.Commit(), particles.Count);

        public void RenderInstanced(GraphicsDevice device, VertexBuffer instances, int count)
        {
            device.SetVertexBuffers(new VertexBufferBinding(this.Vertices), new VertexBufferBinding(instances, 0, 1));
            device.Indices = this.Indices;

            device.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 2, count);
        }

        public void Dispose()
        {
            this.Vertices.Dispose();
            this.Indices.Dispose();
        }
    }
}
