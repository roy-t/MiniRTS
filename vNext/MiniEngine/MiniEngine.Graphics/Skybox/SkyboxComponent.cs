using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Skybox
{
    public sealed class SkyboxComponent : AComponent
    {
        public SkyboxComponent(Entity entity, Texture2D texture, SkyboxVertex[] vertices, short[] indices)
            : base(entity)
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
