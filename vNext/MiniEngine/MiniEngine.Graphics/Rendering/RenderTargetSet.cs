using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Rendering
{
    public sealed class RenderTargetSet
    {
        private readonly GraphicsDevice Device;

        public RenderTargetSet(GraphicsDevice device, int width, int height)
        {
            this.Device = device;

            this.Diffuse = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24);
            this.Normal = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.Color, DepthFormat.None);
            this.Depth = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.Single, DepthFormat.None);

            this.Resolve = new RenderTarget2D(this.Device, width, height, false, SurfaceFormat.Color, DepthFormat.None);
        }

        public RenderTarget2D Diffuse { get; }
        public RenderTarget2D Normal { get; }
        public RenderTarget2D Depth { get; }


        public RenderTarget2D Resolve { get; }
    }
}
