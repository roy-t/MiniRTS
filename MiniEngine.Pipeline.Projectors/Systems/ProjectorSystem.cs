using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class ProjectorSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly EntityLinker EntityLinker;

        public ProjectorSystem(GraphicsDevice device, EntityLinker entityLinker)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
        }

        public void RenderProjectors(IViewPoint viewPoint, GBuffer gBuffer)
        {

        }
    }
}
