using MiniEngine.Systems;

namespace MiniEngine.Graphics.Visibility
{
    public sealed class BoundingSphereComponent : AComponent
    {
        public BoundingSphereComponent(Entity entity, float radius)
            : base(entity)
        {
            this.Radius = radius;
        }

        public float Radius { get; set; }
    }
}
