using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;
using System;

namespace MiniEngine.Pipeline.Shadows.Components
{
    public sealed class CascadedShadowMap : IComponent
    {
        public CascadedShadowMap(GraphicsDevice device, int resolution, int cascades)
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
        }
        
        public int Cascades { get; }

        public RenderTarget2D DepthMapArray { get; }
        public RenderTarget2D ColorMapArray { get; }                

        public string Describe() => $"cascaded shadow map, cascades: {this.Cascades}";
    }
}
