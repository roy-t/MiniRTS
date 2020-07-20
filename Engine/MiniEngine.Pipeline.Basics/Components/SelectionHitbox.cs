using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Basics.Components
{
    public sealed class SelectionHitbox : IComponent
    {
        public SelectionHitbox(Entity entity, float width, float height, float depth)
        {
            this.Entity = entity;
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
        }

        public Entity Entity { get; }

        public Vector2 Min { get; set; }
        public Vector2 Max { get; set; }

        public float Width { get; }
        public float Height { get; }

        public float Depth { get; }
    }
}
