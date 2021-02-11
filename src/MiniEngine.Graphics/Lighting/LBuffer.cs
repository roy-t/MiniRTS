using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class LBuffer
    {
        public LBuffer(GraphicsDevice device)
        {
            this.Light = RenderTargetBuilder.Build(nameof(this.Light), device, SurfaceFormat.HalfVector4);
        }

        public RenderTarget2D Light { get; }
    }
}
