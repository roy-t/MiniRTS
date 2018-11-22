using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Factories
{
    public abstract class AComponentFactory<T> : IComponentFactory
        where T : IComponent
    {
        protected readonly GraphicsDevice Device;
        protected readonly EntityLinker Linker;

        protected AComponentFactory(GraphicsDevice device, EntityLinker linker)
        {
            this.Device = device;
            this.Linker = linker;
        }

        public virtual void Deconstruct(Entity entity) 
            => this.Linker.RemoveComponents<T>(entity);
    }
}
