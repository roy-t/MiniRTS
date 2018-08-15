using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;

namespace MiniEngine.Rendering.Components
{
    public sealed class ShadowMap
    {        
        public ShadowMap(GraphicsDevice device, int depthMapResolution, IViewPoint viewPoint)
        {
            this.ViewPoint = viewPoint;
            this.DepthMap = new RenderTarget2D(device, depthMapResolution, depthMapResolution, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
        }

        public RenderTarget2D DepthMap { get; }
        public IViewPoint ViewPoint { get; }
    }
}
