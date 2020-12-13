using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.PostProcess;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class BrdfLutGenerator
    {
        private const int resolution = 512;

        private readonly GraphicsDevice Device;
        private readonly PostProcessTriangle PostProcessTriangleullScreenTriangle;
        private readonly BrdfLutGeneratorEffect Effect;

        public BrdfLutGenerator(GraphicsDevice device, PostProcessTriangle postProcessTriangle, BrdfLutGeneratorEffect effect)
        {
            this.Device = device;
            this.PostProcessTriangleullScreenTriangle = postProcessTriangle;
            this.Effect = effect;
        }

        public Texture2D Generate()
        {
            var brdfLut = new RenderTarget2D(this.Device, resolution, resolution, false, SurfaceFormat.HalfVector2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SetRenderTarget(brdfLut);
            this.Device.Clear(Color.Black);

            this.Effect.Apply();
            this.PostProcessTriangleullScreenTriangle.Render(this.Device);

            this.Device.SetRenderTarget(null);

            return brdfLut;
        }
    }
}
