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

        public override string ToString() => $"shadow casting light, direction: {this.ViewPoint.LookAt - this.ViewPoint.Position}, color: {this.Color}";
    }
}