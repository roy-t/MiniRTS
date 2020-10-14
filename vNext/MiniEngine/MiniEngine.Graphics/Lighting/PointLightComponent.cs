using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class PointLightComponent : AComponent
    {
        public PointLightComponent(Entity entity, Vector3 position, Color color)
            : base(entity)
        {
            this.Position = position;
            this.Color = color;
        }

        public Vector3 Position { get; set; }
        public Color Color { get; set; }
    }
}
