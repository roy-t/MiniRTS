using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Utilities;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Shadows.Components
{
    public sealed class ShadowMap
    {
        public ShadowMap(RenderTarget2D depthMapArray, RenderTarget2D colorMapArray, int index, IViewPoint viewPoint)
        {            
            this.DepthMap = depthMapArray;
            this.ColorMap = colorMapArray;
            this.Index = index;
            this.ViewPoint = viewPoint;
        }

        public ShadowMap(GraphicsDevice device, int resolution, IViewPoint viewPoint)
        {
            this.ViewPoint = viewPoint;
            this.DepthMap = new RenderTarget2D(
                device,
                resolution,
                resolution,
                false,
                SurfaceFormat.Single,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.DiscardContents,
                false);

            this.ColorMap = new RenderTarget2D(
                device,
                resolution,
                resolution,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents,
                false);

            this.Index = 0;
        }
                
        public RenderTarget2D DepthMap { get; }
        public RenderTarget2D ColorMap { get; }
        public int Index { get; }
        public IViewPoint ViewPoint { get; }
    }
}