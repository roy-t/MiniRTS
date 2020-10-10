using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Rendering
{
    public sealed class RenderTargetSet
    {
        private readonly GraphicsDevice Device;

        public RenderTargetSet(GraphicsDevice device, int width, int height)
        {
            this.Device = device;

            this.Diffuse = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.ColorSRgb, DepthFormat.Depth24);
            this.Depth = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.Single, DepthFormat.None);
            this.Normal = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.Color, DepthFormat.None);

            this.Combine = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.ColorSRgb, DepthFormat.None);
            this.PostProcess = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.ColorSRgb, DepthFormat.None);
        }

        public RenderTarget2D Diffuse { get; }

        public RenderTarget2D Depth { get; }

        public RenderTarget2D Normal { get; }

        public RenderTarget2D Combine { get; }

        public RenderTarget2D PostProcess { get; }
    }
}
