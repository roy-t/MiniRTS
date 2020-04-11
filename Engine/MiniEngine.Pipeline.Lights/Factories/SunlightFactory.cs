using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Shadows.Factories;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class SunlightFactory : AComponentFactory<Sunlight>
    {
        private const int DefaultCascades = 4;
        private const int DefaultResolution = 2048;

        private readonly CascadedShadowMapFactory CascadedShadowMapFactory;

        public SunlightFactory(GraphicsDevice device, IComponentContainer<Sunlight> container, CascadedShadowMapFactory cascadedShadowMapFactory)
            : base(device, container)
        {
            this.CascadedShadowMapFactory = cascadedShadowMapFactory;
        }

        public Sunlight Construct(Entity entity, Color color, Vector3 position, Vector3 lookAt, int cascades = DefaultCascades, int resolution = DefaultResolution)
        {
            var shadowMap = this.CascadedShadowMapFactory.Construct(entity, position, lookAt, cascades, resolution);

            var light = new Sunlight(entity, shadowMap, color);
            this.Container.Add(entity, light);

            return light;
        }
    }
}
