using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class ShadowCastingLight : IComponent
    {
        public ShadowCastingLight(PerspectiveCamera viewPoint, ShadowMap shadowMap, Color color)
        {
            this.ViewPoint = viewPoint;
            this.ShadowMap = shadowMap;
            this.Color = color;
        }

        public PerspectiveCamera ViewPoint { get; }
        public ShadowMap ShadowMap { get; }
        public Color Color { get; set; }

        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Shadow casting light");
            description.AddProperty("Color", this.Color, x => this.Color = x, MinMaxDescription.ZeroToOne);
            description.AddProperty("Position", this.ViewPoint.Position, x => this.ViewPoint.Move(x, this.ViewPoint.LookAt), MinMaxDescription.MinusInfinityToInfinity);
            description.AddProperty("Look At", this.ViewPoint.LookAt, x => this.ViewPoint.Move(this.ViewPoint.Position, x), MinMaxDescription.MinusInfinityToInfinity);
            
            return description;
        }

        public override string ToString() => $"shadow casting light, direction: {this.ViewPoint.LookAt - this.ViewPoint.Position}, color: {this.Color}";
    }
}