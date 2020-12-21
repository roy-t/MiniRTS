using System;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Skybox
{
    public sealed class SkyboxGeometry : IDisposable
    {
        public SkyboxGeometry(TextureCube texture, TextureCube irradiance, TextureCube environment, VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            this.Texture = texture;
            this.Irradiance = irradiance;
            this.Environment = environment;

            this.VertexBuffer = vertexBuffer;
            this.IndexBuffer = indexBuffer;

            this.Primitives = this.IndexBuffer.IndexCount / 3;

            this.AmbientLightFactor = 1.0f;
        }

        public TextureCube Texture { get; set; }

        public TextureCube Irradiance { get; set; }

        public TextureCube Environment { get; set; }
        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }

        public float AmbientLightFactor { get; set; }


        public int Primitives { get; }

        public void Dispose()
        {
            this.VertexBuffer?.Dispose();
            this.IndexBuffer?.Dispose();
        }
    }
}
