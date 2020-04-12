using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Basics.Components
{
    public sealed class Bounds : IComponent
    {
        public Bounds(Entity entity, BoundingSphere neutralBoundingSphere)
        {
            this.Entity = entity;
            this.NeutralBoundingSphere = neutralBoundingSphere;
        }

        public Entity Entity { get; }

        public BoundingSphere NeutralBoundingSphere { get; }

        public BoundingSphere BoundingSphere { get; internal set; }

        public bool IsInView { get; internal set; }
    }
}
