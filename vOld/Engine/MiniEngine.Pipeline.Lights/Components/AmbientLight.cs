using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class AmbientLight : IComponent
    {
        public AmbientLight(Entity entity, Color color)
        {
            this.Entity = entity;
            this.Color = color;
        }

        public Entity Entity { get; }

        [Editor(nameof(Color))]
        public Color Color { get; set; }
    }
}
