using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Rendering
{
    public sealed class RenderTargetSet
    {
        public RenderTargetSet(GraphicsDevice device)
        {
            this.Diffuse = BuildRenderTarget(device, SurfaceFormat.ColorSRgb, DepthFormat.Depth24);
            this.Material = BuildRenderTarget(device, SurfaceFormat.Color);
            this.Depth = BuildRenderTarget(device, SurfaceFormat.Single);
            this.Normal = BuildRenderTarget(device, SurfaceFormat.Color);
            this.Light = BuildRenderTarget(device, SurfaceFormat.HalfVector4);
            this.Combine = BuildRenderTarget(device, SurfaceFormat.ColorSRgb);
            this.PostProcess = BuildRenderTarget(device, SurfaceFormat.ColorSRgb);
        }

        private static RenderTarget2D BuildRenderTarget(GraphicsDevice device, SurfaceFormat surface, DepthFormat depthFormat = DepthFormat.None)
        {
            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;
            return new RenderTarget2D(device, width, height, false, surface, depthFormat, 0, RenderTargetUsage.PreserveContents);
        }

        public RenderTarget2D Diffuse { get; }

        public RenderTarget2D Material { get; }

        public RenderTarget2D Depth { get; }

        public RenderTarget2D Normal { get; }

        public RenderTarget2D Light { get; }

        public RenderTarget2D Combine { get; }

        public RenderTarget2D PostProcess { get; }
    }
}
