using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Shadows.Components
{
    public sealed class CascadedShadowMap
    {
        public CascadedShadowMap(GraphicsDevice device, int resolution, int cascades, Reference<IViewPoint>[] viewPoints)
        {
            this.Cascades = cascades;

            this.DepthMapArray = new RenderTarget2D(
                device,
                resolution,
                resolution,
                false,
                SurfaceFormat.Single,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.DiscardContents,
                false,
                cascades);

            this.ColorMapArray = new RenderTarget2D(
                device,
                resolution,
                resolution,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents,
                false,
                cascades);

            this.ShadowMaps = new ShadowMap[cascades];
            for(var i = 0; i < cascades; i++)
            {
                this.ShadowMaps[i] = new ShadowMap(this.DepthMapArray, this.ColorMapArray, i, viewPoints[i]);
            }
        }
        
        public int Cascades { get; }
        public ShadowMap[] ShadowMaps { get; }

        public RenderTarget2D DepthMapArray { get; }
        public RenderTarget2D ColorMapArray { get; }
    }
}
