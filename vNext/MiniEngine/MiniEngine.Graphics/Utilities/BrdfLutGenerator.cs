using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
using MiniEngine.Graphics.PostProcess;

namespace MiniEngine.Graphics.Utilities
{
    [Service]
    public sealed class BrdfLutGenerator
    {
        private const int resolution = 512;

        private readonly GraphicsDevice Device;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly BrdfLutGeneratorEffect Effect;

        public BrdfLutGenerator(GraphicsDevice device, FullScreenTriangle fullScreenTriangle, EffectFactory effectFactory)
        {
            this.Device = device;
            this.FullScreenTriangle = fullScreenTriangle;

            this.Effect = effectFactory.Construct<BrdfLutGeneratorEffect>();
        }

        public Texture2D Generate()
        {
            // TODO: maybe we do not need preserve?
            var brdfLut = new RenderTarget2D(this.Device, resolution, resolution, false, SurfaceFormat.HalfVector2, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            this.Device.BlendState = BlendState.Opaque;
            this.Device.DepthStencilState = DepthStencilState.None;
            this.Device.RasterizerState = RasterizerState.CullCounterClockwise;

            this.Device.SetRenderTarget(brdfLut);
            this.Device.Clear(Color.Black);

            this.Effect.Apply();
            this.FullScreenTriangle.Render(this.Device);

            this.Device.SetRenderTarget(null);

            return brdfLut;
        }
    }
}
