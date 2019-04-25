using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    [Label(nameof(ShadowCastingLight))]
    public sealed class ShadowCastingLight : IComponent
    {
        public ShadowCastingLight(PerspectiveCamera viewPoint, ShadowMap shadowMap, Color color)
        {
            this.ViewPoint = viewPoint;
            this.ShadowMap = shadowMap;
            this.Color = color;
        }

        [Editor(nameof(ViewPoint))]
        public PerspectiveCamera ViewPoint { get; set; }
        
        public ShadowMap ShadowMap { get; }

        [Editor(nameof(Color))]
        public Color Color { get; set; }       
    }
}