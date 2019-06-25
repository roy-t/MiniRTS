using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Shadows.Factories
{
    public sealed class ShadowMapFactory : AComponentFactory<ShadowMap>
    {
        private const int DefaultResolution = 1024;

        public ShadowMapFactory(GraphicsDevice device, EntityLinker linker)
            : base(device, linker) { }

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
            this.Linker.AddComponent(entity, shadowMap);

            return shadowMap;
        }

        internal ShadowMap Construct(Entity entity, RenderTarget2D depthMapArray, RenderTarget2D colorMapArray, int index, IViewPoint viewPoint)
        {
            var shadowMap = new ShadowMap(entity, depthMapArray, colorMapArray, index, viewPoint);
            this.Linker.AddComponent(entity, shadowMap);

            return shadowMap;
        }
    }
}
