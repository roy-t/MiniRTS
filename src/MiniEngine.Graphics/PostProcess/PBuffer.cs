using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class PBuffer
    {
        public PBuffer(GraphicsDevice device)
        {
            this.ToneMap = RenderTargetBuilder.Build(nameof(this.ToneMap), device, SurfaceFormat.ColorSRgb);
        }

        public RenderTarget2D ToneMap { get; }
    }
}
