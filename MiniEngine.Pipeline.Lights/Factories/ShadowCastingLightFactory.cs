using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Shadows.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class ShadowCastingLightFactory : AComponentFactory<ShadowCastingLight>
    {
        private const int DefaultResolution = 1024;


        private readonly ShadowMapFactory ShadowMapFactory;

        public ShadowCastingLightFactory(GraphicsDevice device, EntityLinker linker, ShadowMapFactory shadowMapFactory)
            : base(device, linker)
        {
            this.ShadowMapFactory = shadowMapFactory;
        }

        public void Construct(Entity entity, Vector3 position, Vector3 lookAt, Color color, int resolution = DefaultResolution)
        {
            var viewPoint = new PerspectiveCamera(new Viewport(0, 0, resolution, resolution));
            viewPoint.Move(position, lookAt);

            var shadowMap = this.ShadowMapFactory.Construct(entity, viewPoint, resolution);
            var light = new ShadowCastingLight(viewPoint, shadowMap, color);
            this.Linker.AddComponent(entity, light);
        }
    }
}
