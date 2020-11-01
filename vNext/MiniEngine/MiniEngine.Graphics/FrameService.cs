using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Skybox;

namespace MiniEngine.Graphics
{
    [Service]
    public sealed class FrameService
    {
        public FrameService(GraphicsDevice device)
        {
            this.Skybox = null!;
            this.BrdfLutTexture = null!;
            this.Camera = new PerspectiveCamera(device.Viewport.AspectRatio);
            this.GBuffer = new GBuffer(device);
            this.LBuffer = new LBuffer(device);
            this.PBuffer = new PBuffer(device);
        }

        public ICamera Camera { get; set; }
        public GBuffer GBuffer { get; set; }
        public LBuffer LBuffer { get; set; }
        public PBuffer PBuffer { get; set; }

        public SkyboxGeometry Skybox { get; set; } // TODO: this field should be move to a scene object and initialized better!

        public Texture2D BrdfLutTexture { get; set; } // TODO: this is a general shader resource that should be somewhere else?
    }
}
