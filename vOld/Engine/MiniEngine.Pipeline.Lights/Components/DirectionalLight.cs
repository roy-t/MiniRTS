using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class DirectionalLight : IComponent
    {
        public DirectionalLight(Entity entity, Vector3 direction, Color color)
        {
            this.Entity = entity;
            this.Direction = direction;
            this.Color = color;
        }

        public Entity Entity { get; }

        [Editor(nameof(Direction), nameof(Direction), -1, 1)]
        public Vector3 Direction { get; set; }

        [Editor(nameof(Color))]
        public Color Color { get; set; }        
    }
}