using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics.PostProcess
{
    public sealed class PBuffer
    {
        public PBuffer(GraphicsDevice device)
        {
            this.Combine = RenderTargetBuilder.Build(device, SurfaceFormat.ColorSRgb);
            this.PostProcess = RenderTargetBuilder.Build(device, SurfaceFormat.ColorSRgb);
        }

        public RenderTarget2D Combine { get; }

        public RenderTarget2D PostProcess { get; }
    }
}
