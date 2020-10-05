using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics
{
    public sealed class FrameService
    {
        public FrameService(GraphicsDevice device)
        {
            this.Camera = new PerspectiveCamera(device.Viewport.AspectRatio);
            this.RenderTargetSet = new RenderTargetSet(device, device.Viewport.Width, device.Viewport.Height);
        }

        public ICamera Camera { get; set; }
        public RenderTargetSet RenderTargetSet { get; set; }
    }
}
