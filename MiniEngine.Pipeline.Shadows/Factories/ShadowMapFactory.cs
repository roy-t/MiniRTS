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
            var shadowMap = new ShadowMap(this.Device, resolution, viewPoint);
            this.Linker.AddComponent(entity, shadowMap);

            return shadowMap;
        }

        internal ShadowMap Construct(Entity entity, RenderTarget2D depthMapArray, RenderTarget2D colorMapArray, int index, IViewPoint viewPoint)
        {
            var shadowMap = new ShadowMap(depthMapArray, colorMapArray, index, viewPoint);
            this.Linker.AddComponent(entity, shadowMap);

            return shadowMap;
        }
    }
}
