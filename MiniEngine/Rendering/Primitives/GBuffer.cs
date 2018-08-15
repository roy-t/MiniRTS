using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Primitives
{
    public sealed class GBuffer
    {
        public GBuffer(GraphicsDevice device, int width, int height)
        {
            // Do not enable AA as we use FXAA during post processing
            const int aaSamples = 0;

            this.ColorTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.Depth24,
                aaSamples,
                RenderTargetUsage.DiscardContents);

            this.NormalTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.DiscardContents);

            this.DepthTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Single,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.DiscardContents);

            this.LightTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.DiscardContents);

         
        }

        public RenderTarget2D ColorTarget { get; }
        public RenderTarget2D NormalTarget { get; }
        public RenderTarget2D DepthTarget { get; }
        public RenderTarget2D LightTarget { get; }        
    }
}
