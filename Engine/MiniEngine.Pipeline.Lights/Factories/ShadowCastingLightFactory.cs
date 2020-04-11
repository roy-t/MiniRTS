using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Shadows.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class ShadowCastingLightFactory : AComponentFactory<ShadowCastingLight>
    {
        private const int DefaultResolution = 1024;


        private readonly ShadowMapFactory ShadowMapFactory;

        public ShadowCastingLightFactory(GraphicsDevice device, IComponentContainer<ShadowCastingLight> container, ShadowMapFactory shadowMapFactory)
            : base(device, container)
        {
            this.ShadowMapFactory = shadowMapFactory;
        }

        public ShadowCastingLight Construct(Entity entity, Vector3 position, Vector3 lookAt, Color color, int resolution = DefaultResolution)
        {
            var viewPoint = new PerspectiveCamera(new Viewport(0, 0, resolution, resolution));
            viewPoint.Move(position, lookAt);

            var shadowMap = this.ShadowMapFactory.Construct(entity, viewPoint, resolution);
            var light = new ShadowCastingLight(entity, viewPoint, shadowMap, color);
            this.Container.Add(entity, light);

            return light;
        }
    }
}
