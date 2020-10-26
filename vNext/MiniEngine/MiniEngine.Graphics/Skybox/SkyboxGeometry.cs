using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Skybox
{
    public sealed class SkyboxGeometry
    {
        public SkyboxGeometry(Texture2D texture, SkyboxVertex[] vertices, short[] indices)
        {
            this.Texture = texture;
            this.Vertices = vertices;
            this.Indices = indices;

            this.Primitives = this.Indices.Length / 3;
        }

        public Texture2D Texture { get; set; }

        public SkyboxVertex[] Vertices { get; }

        public short[] Indices { get; }

        public int Primitives { get; }
    }
}
