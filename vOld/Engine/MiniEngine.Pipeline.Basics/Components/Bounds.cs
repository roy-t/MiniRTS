using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Basics.Components
{
    public sealed class Bounds : IComponent
    {
        public Bounds(Entity entity, Vector3 min, Vector3 max)
        {
            this.Entity = entity;
            this.NeutralBoundingBox = new BoundingBox(min, max);
        }

        public Entity Entity { get; }
        public Vector3 Center => Vector3.Lerp(this.BoundingBox.Min, this.BoundingBox.Max, 0.5f);
        public BoundingBox NeutralBoundingBox { get; }
        public BoundingBox BoundingBox { get; internal set; }
        public bool IsInView { get; internal set; }
    }
}
