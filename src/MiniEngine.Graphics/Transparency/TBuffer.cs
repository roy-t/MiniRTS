using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics.Transparency
{
    public sealed class TBuffer
    {
        public TBuffer(GraphicsDevice device)
        {
            this.Albedo = RenderTargetBuilder.Build(nameof(this.Albedo), device, SurfaceFormat.HalfVector4, DepthFormat.None);
            this.Weights = RenderTargetBuilder.Build(nameof(this.Weights), device, SurfaceFormat.HalfSingle, DepthFormat.None);
        }

        public RenderTarget2D Albedo { get; }

        public RenderTarget2D Weights { get; }
    }
}
