using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    // TODO: what if we replace the different light types with one light component
    // they are all the same anyways, and then render the lights differently
    // based on its entities other components?

    public sealed class SunlightComponent : AComponent
    {
        public SunlightComponent(Entity entity, Color color, float strength)
            : base(entity)
        {
            this.Color = color;
            this.Strength = strength;
        }

        public Color Color { get; set; }

        public float Strength { get; set; }
    }
}
