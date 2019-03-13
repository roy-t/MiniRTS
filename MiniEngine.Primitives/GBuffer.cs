using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Primitives
{
    public sealed class GBuffer
    {
        public GBuffer(GraphicsDevice device, int width, int height)
        {
            // Do not enable AA as we use FXAA during post processing
            const int aaSamples = 0;

            this.DiffuseTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.HalfVector4,
                DepthFormat.Depth24,
                aaSamples,
                RenderTargetUsage.PreserveContents);

            this.NormalTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.PreserveContents);

            this.DepthTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Single,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.PreserveContents);

            this.ParticleTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.HalfSingle,
                DepthFormat.None,
                aaSamples,
                RenderTargetUsage.PreserveContents);

            this.LightTarget = new RenderTarget2D(
                device,
                width,
                height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            this.CombineTarget = new RenderTarget2D(
              device,
              width,
              height,
              false,
              SurfaceFormat.Color,
              DepthFormat.None,
              0,
              RenderTargetUsage.PreserveContents);

            this.BlurTarget = new RenderTarget2D(
               device,
               width,
               height,
               false,
               SurfaceFormat.Color,
               DepthFormat.None,
               0,
               RenderTargetUsage.PreserveContents);

            this.FinalTarget = new RenderTarget2D(
              device,
              width,
              height,
              false,
              SurfaceFormat.Color,
              DepthFormat.None,
              0,
              RenderTargetUsage.PreserveContents);

            
        }

        public RenderTarget2D DiffuseTarget { get; }
        public RenderTarget2D NormalTarget { get; }
        public RenderTarget2D ParticleTarget { get; }
        public RenderTarget2D DepthTarget { get; }
        public RenderTarget2D LightTarget { get; }
        public RenderTarget2D BlurTarget { get; }
        public RenderTarget2D CombineTarget { get; }
        public RenderTarget2D FinalTarget { get; }        
    }
}