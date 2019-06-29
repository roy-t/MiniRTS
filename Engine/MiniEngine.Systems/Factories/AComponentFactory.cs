using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Systems.Factories
{
    public abstract class AComponentFactory<T> : IComponentFactory
        where T : IComponent
    {
        protected readonly GraphicsDevice Device;
        protected readonly IComponentContainer<T> Container;

        protected AComponentFactory(GraphicsDevice device, IComponentContainer<T> container)
        {
            this.Device = device;
            this.Container = container;
        }

        public virtual void Deconstruct(Entity entity) => this.Container.RemoveAllOwnedBy(entity);
    }
}
