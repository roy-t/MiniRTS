using MiniEngine.Configuration;

namespace MiniEngine.Systems
{
    [Component]
    public abstract class AComponent
    {
        protected AComponent(Entity entity)
        {
            this.Entity = entity;
        }

        public Entity Entity { get; }
    }
}
