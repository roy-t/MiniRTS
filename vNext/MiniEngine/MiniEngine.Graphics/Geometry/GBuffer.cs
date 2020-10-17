using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class GBuffer
    {
        public GBuffer(GraphicsDevice device)
        {
            this.Diffuse = RenderTargetBuilder.Build(device, SurfaceFormat.ColorSRgb, DepthFormat.Depth24);
            this.Material = RenderTargetBuilder.Build(device, SurfaceFormat.Color);
            this.Depth = RenderTargetBuilder.Build(device, SurfaceFormat.Single);
            this.Normal = RenderTargetBuilder.Build(device, SurfaceFormat.Color);
        }

        public RenderTarget2D Diffuse { get; }

        public RenderTarget2D Material { get; }

        public RenderTarget2D Depth { get; }

        public RenderTarget2D Normal { get; }


    }
}
