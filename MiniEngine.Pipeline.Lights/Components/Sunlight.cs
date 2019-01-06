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

        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Sun light");
            description.AddProperty("Color", this.Color, x => this.Color = x, 0.0f, 1.0f);
            description.AddProperty("Position", this.ShadowMapCascades.Position, x => this.ShadowMapCascades.Move(x, this.ShadowMapCascades.LookAt), -100.0f, 100.0f);
            description.AddProperty("Look At", this.ShadowMapCascades.LookAt, x => this.ShadowMapCascades.Move(this.ShadowMapCascades.Position, x), -100.0f, 100.0f);

            return description;
        }

        public override string ToString() => $"sun light, color {this.Color}";
    }
}