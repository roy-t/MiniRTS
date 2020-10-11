using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class AmbientLightComponent : AComponent
    {
        public AmbientLightComponent(Entity entity, Color color)
            : base(entity)
        {
            this.Color = color;
        }

        public Color Color { get; set; }
    }
}
