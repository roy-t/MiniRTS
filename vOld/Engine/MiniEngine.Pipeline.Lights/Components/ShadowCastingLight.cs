using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    // TODO: use Pose 
    public sealed class ShadowCastingLight : IComponent
    {
        public ShadowCastingLight(Entity entity, PerspectiveCamera viewPoint, ShadowMap shadowMap, Color color)
        {
            this.Entity = entity;
            this.ViewPoint = viewPoint;
            this.ShadowMap = shadowMap;
            this.Color = color;
        }

        public Entity Entity { get; }

        [Editor(nameof(ViewPoint))]
        public PerspectiveCamera ViewPoint { get; set; }

        public ShadowMap ShadowMap { get; }

        [Editor(nameof(Color))]
        public Color Color { get; set; }
    }
}