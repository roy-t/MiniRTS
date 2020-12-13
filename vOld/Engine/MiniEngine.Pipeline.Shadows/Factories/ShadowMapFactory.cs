using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Shadows.Factories
{
    public sealed class ShadowMapFactory : AComponentFactory<ShadowMap>
    {
        private const int DefaultResolution = 1024;

        public ShadowMapFactory(GraphicsDevice device, IComponentContainer<ShadowMap> container)
            : base(device, container) { }

        public ShadowMap Construct(Entity entity, IViewPoint viewPoint, int resolution = DefaultResolution)
        {
            var depthMap = new RenderTarget2D(
               this.Device,
               resolution,
               resolution,
               false,
               SurfaceFormat.Single,
               DepthFormat.Depth24,
               0,
               RenderTargetUsage.DiscardContents,
               false);

            var colorMap = new RenderTarget2D(
                this.Device,
                resolution,
                resolution,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents,
                false);


            var shadowMap = new ShadowMap(entity, depthMap, colorMap, 0, viewPoint);
            this.Container.Add(shadowMap);

            return shadowMap;
        }
    }
}
