using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GBuffer
    {
        public GBuffer(GraphicsDevice device)
        {
            this.Albedo = RenderTargetBuilder.Build(device, SurfaceFormat.ColorSRgb, DepthFormat.Depth24);
            this.Material = RenderTargetBuilder.Build(device, SurfaceFormat.Color);
            this.Depth = RenderTargetBuilder.Build(device, SurfaceFormat.Single);
            this.Normal = RenderTargetBuilder.Build(device, SurfaceFormat.HalfVector4);
        }

        public RenderTarget2D Albedo { get; }

        public RenderTarget2D Material { get; }

        public RenderTarget2D Depth { get; }

        public RenderTarget2D Normal { get; }
    }
}
