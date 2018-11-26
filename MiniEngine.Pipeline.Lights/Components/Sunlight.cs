using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class Sunlight : IComponent
    {
        public Sunlight(CascadedShadowMap shadowMapCascades, Color color)
        {
            this.ShadowMapCascades = shadowMapCascades;
            this.Color = color;
        }

        public CascadedShadowMap ShadowMapCascades { get; }
        public Color Color { get; set; }

        public override string ToString() => $"sun light, color {this.Color}";
    }
}