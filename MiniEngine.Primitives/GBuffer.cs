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

        public long ComputeSize()
        {
            var targets = new[] { this.DiffuseTarget, this.NormalTarget, this.ParticleTarget, this.DepthTarget, this.LightTarget, this.BlurTarget, this.CombineTarget, this.FinalTarget };
            var size = 0L;
            var sizeMb = 0L;

            foreach (var target in targets)
            {                
                for(var i = 0; i < target.LevelCount; i++)
                {
                    var width = target.Width * 1 / (i + 1);
                    var height = target.Height * 1 / (i + 1);
                    var pixels = width * height;
                    var colorBytesPerPixel = GetBitsPerPixel(target.Format) / 8;
                    var depthBytesPerPixel = GetBitsPerPixel(target.DepthStencilFormat) / 8;
                    var targetSize = (pixels * colorBytesPerPixel) + (pixels * depthBytesPerPixel);

                    size += targetSize;
                    sizeMb += (targetSize / (1024 * 1024));
                }                
            }

            return size;
        }

        private static int GetBitsPerPixel(DepthFormat format)
        {
            switch (format)
            {
                case DepthFormat.Depth16:
                    return 16;
                case DepthFormat.Depth24:
                    return 24;
                case DepthFormat.Depth24Stencil8:
                    return 32;

                case DepthFormat.None:
                default:
                    return 0;
            }
        }

        private static int GetBitsPerPixel(SurfaceFormat format)
        {
            switch (format)
            {
                case SurfaceFormat.Color:
                    return 32;
                case SurfaceFormat.Bgr565:
                    return 16;
                case SurfaceFormat.Bgra5551:
                    return 16;
                case SurfaceFormat.Bgra4444:
                    return 16;
                case SurfaceFormat.Dxt1:
                    return 0;
                case SurfaceFormat.Dxt3:
                    return 0;
                case SurfaceFormat.Dxt5:
                    return 0;
                case SurfaceFormat.NormalizedByte2:
                    return 16;
                case SurfaceFormat.NormalizedByte4:
                    return 32;
                case SurfaceFormat.Rgba1010102:
                    return 32;
                case SurfaceFormat.Rg32:
                    return 32;
                case SurfaceFormat.Rgba64:
                    return 32;
                case SurfaceFormat.Alpha8:
                    return 8;
                case SurfaceFormat.Single:
                    return 32;
                case SurfaceFormat.Vector2:
                    return 64;
                case SurfaceFormat.Vector4:
                    return 128;
                case SurfaceFormat.HalfSingle:
                    return 16;
                case SurfaceFormat.HalfVector2:
                    return 32;
                case SurfaceFormat.HalfVector4:
                    return 64;
                case SurfaceFormat.HdrBlendable:
                    return 0;
                case SurfaceFormat.Bgr32:
                    return 32;
                case SurfaceFormat.Bgra32:
                    return 32;
                case SurfaceFormat.ColorSRgb:
                    return 32;
                case SurfaceFormat.Bgr32SRgb:
                    return 32;
                case SurfaceFormat.Bgra32SRgb:
                    return 32;
                case SurfaceFormat.Dxt1SRgb:
                    return 0;
                case SurfaceFormat.Dxt3SRgb:
                    return 0;
                case SurfaceFormat.Dxt5SRgb:
                    return 0;
                case SurfaceFormat.RgbPvrtc2Bpp:
                    return 0;
                case SurfaceFormat.RgbPvrtc4Bpp:
                    return 0;
                case SurfaceFormat.RgbaPvrtc2Bpp:
                    return 0;
                case SurfaceFormat.RgbaPvrtc4Bpp:
                    return 0;
                case SurfaceFormat.RgbEtc1:
                    return 0;
                case SurfaceFormat.Dxt1a:
                    return 0;
                case SurfaceFormat.RgbaAtcExplicitAlpha:
                    return 0;
                case SurfaceFormat.RgbaAtcInterpolatedAlpha:
                    return 0;
            }
            return 0;
        }
    }
}