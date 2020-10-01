using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Systems;

namespace MiniEngine.Graphics
{
    [System]
    public sealed class ClearSystem : ISystem
    {
        private readonly GraphicsDevice GraphicsDevice;

        public ClearSystem(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
        }

        public void Process() => this.GraphicsDevice.Clear(Color.Purple);
    }
}
