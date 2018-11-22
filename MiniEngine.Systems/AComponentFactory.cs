using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Systems
{
    public abstract class AComponentFactory
    {
        protected readonly GraphicsDevice Device;
        protected readonly EntityLinker Linker;

        protected AComponentFactory(GraphicsDevice device, EntityLinker linker)
        {
            this.Device = device;
            this.Linker = linker;
        }        
    }
}
