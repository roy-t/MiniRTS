using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class PointLightComponent : AComponent
    {
        public PointLightComponent(Entity entity, Vector3 position, Color color, float strength)
            : base(entity)
        {
            this.Position = position;
            this.Color = color;
            this.Strength = strength;
        }

        public Vector3 Position { get; set; }
        public Color Color { get; set; }

        public float Strength { get; set; }
    }
}
