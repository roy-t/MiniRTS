using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.ContentPipeline.Shared
{
    public sealed class GeometryData : IDisposable
    {
        public GeometryData(VertexBuffer vertexBuffer, IndexBuffer indexBuffer, BoundingSphere bounds)
        {
            this.VertexBuffer = vertexBuffer;
            this.IndexBuffer = indexBuffer;
            this.Bounds = bounds;
            this.Primitives = this.IndexBuffer.IndexCount / 3;
        }

        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }
        public BoundingSphere Bounds { get; }
        public int Primitives { get; }

        public void Dispose()
        {
            this.VertexBuffer.Dispose();
            this.IndexBuffer.Dispose();
        }
    }
}
