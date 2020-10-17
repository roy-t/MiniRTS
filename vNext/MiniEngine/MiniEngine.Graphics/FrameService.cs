using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.PostProcess;

namespace MiniEngine.Graphics
{
    public sealed class FrameService
    {
        public FrameService(GraphicsDevice device)
        {
            this.Camera = new PerspectiveCamera(device.Viewport.AspectRatio);
            this.GBuffer = new GBuffer(device);
            this.LBuffer = new LBuffer(device);
            this.PBuffer = new PBuffer(device);
        }

        public ICamera Camera { get; set; }
        public GBuffer GBuffer { get; set; }
        public LBuffer LBuffer { get; set; }
        public PBuffer PBuffer { get; set; }
    }
}
