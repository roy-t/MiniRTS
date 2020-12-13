using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Rendering
{
    public static class RenderTargetBuilder
    {
        public static RenderTarget2D Build(GraphicsDevice device, SurfaceFormat surface, DepthFormat depthFormat = DepthFormat.None)
        {
            var width = device.PresentationParameters.BackBufferWidth;
            var height = device.PresentationParameters.BackBufferHeight;
            return new RenderTarget2D(device, width, height, false, surface, depthFormat, 0, RenderTargetUsage.PreserveContents);
        }
    }
}
