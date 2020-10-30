using System;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Skybox
{
    public sealed class SkyboxGeometry : IDisposable
    {
        public SkyboxGeometry(TextureCube texture, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            this.Texture = texture;
            this.VertexBuffer = vertexBuffer;
            this.IndexBuffer = indexBuffer;

            this.Primitives = this.IndexBuffer.IndexCount / 3;
        }

        public TextureCube Texture { get; set; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }

        public int Primitives { get; }

        public void Dispose()
        {
            this.VertexBuffer?.Dispose();
            this.IndexBuffer?.Dispose();
        }
    }
}
