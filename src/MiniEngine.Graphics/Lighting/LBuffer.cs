using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Rendering;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class LBuffer
    {
        public LBuffer(GraphicsDevice device)
        {
            this.Light = RenderTargetBuilder.Build(device, SurfaceFormat.HalfVector4);
            this.ParticipatingMedia = RenderTargetBuilder.Build(device, SurfaceFormat.Vector2);
            this.LightPostProcess = RenderTargetBuilder.Build(device, SurfaceFormat.HalfVector4);
        }

        public RenderTarget2D Light { get; }

        public RenderTarget2D ParticipatingMedia { get; }

        public RenderTarget2D LightPostProcess { get; }
    }
}
