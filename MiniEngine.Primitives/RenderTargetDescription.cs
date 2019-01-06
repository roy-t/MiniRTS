using Microsoft.Xna.Framework.Graphics;
using System;

namespace MiniEngine.Primitives
{
    public sealed class RenderTargetDescription : IComparable<RenderTargetDescription>
    {
        public RenderTargetDescription(RenderTarget2D renderTarget, string name, int order)
        {
            this.RenderTarget = renderTarget;
            this.Name = name;
            this.Order = order;
            this.SizeInBytes = ComputeSizeInBytes();
        }

        public RenderTarget2D RenderTarget { get; }
        public string Name { get; }
        public int Order { get; }
        private long SizeInBytes { get; }

        private long ComputeSizeInBytes()
        {
            var size = 0L;

            for (var i = 0; i < this.RenderTarget.LevelCount; i++)
            {
                var width = this.RenderTarget.Width * 1 / (i + 1);
                var height = this.RenderTarget.Height * 1 / (i + 1);
                var pixels = width * height;
                var colorBytesPerPixel = GetBitsPerPixel(this.RenderTarget.Format) / 8;
                var depthBytesPerPixel = GetBitsPerPixel(this.RenderTarget.DepthStencilFormat) / 8;
                var targetSize = (pixels * colorBytesPerPixel) + (pixels * depthBytesPerPixel);

                size += targetSize;
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

        public int CompareTo(RenderTargetDescription other) => this.Order.CompareTo(other.Order);
    }
}
