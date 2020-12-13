using MiniEngine.Configuration;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems
{
    [Component]
    public abstract class AComponent
    {
        protected AComponent(Entity entity)
        {
            this.Entity = entity;
            this.ChangeState = ComponentChangeState.NewComponent();
        }

        public Entity Entity { get; }

        public ComponentChangeState ChangeState { get; }
    }
}
