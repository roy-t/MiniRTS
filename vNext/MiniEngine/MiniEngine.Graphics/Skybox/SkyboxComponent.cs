using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Skybox
{
    public sealed class SkyboxComponent : AComponent
    {
        public SkyboxComponent(Entity entity, Texture2D texture)
            : base(entity)
        {
            this.Texture = texture;
        }

        public Texture2D Texture { get; set; }
    }
}
