using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Shadows.Components
{
    [Label(nameof(ShadowMap))]
    public sealed class ShadowMap : IComponent
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

        [Editor(nameof(Width))]
        public int Width => this.DepthMap.Width;

        [Editor(nameof(Height))]
        public int Height => this.DepthMap.Height;

        [Editor(nameof(DepthMap), null, nameof(Index))]
        public RenderTarget2D DepthMap { get; }

        [Editor(nameof(ColorMap), null, nameof(Index))]
        public RenderTarget2D ColorMap { get; }

        [Editor(nameof(Index))]
        public int Index { get; }

        public IViewPoint ViewPoint { get; }        
    }
}