using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Shadows.Factories;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class SunlightFactory : AComponentFactory<Sunlight>
    {
        private const int DefaultCascades = 4;
        private const int DefaultResolution = 2048;

        private readonly CascadedShadowMapFactory CascadedShadowMapFactory;

        public SunlightFactory(GraphicsDevice device, EntityLinker linker, CascadedShadowMapFactory cascadedShadowMapFactory) 
            : base(device, linker) {
            this.CascadedShadowMapFactory = cascadedShadowMapFactory;
        }

        public void Construct(Entity entity, Color color, Vector3 position, Vector3 lookAt, int cascades = DefaultCascades, int resolution = DefaultResolution)
        {
            var shadowMap = this.CascadedShadowMapFactory.Construct(entity, position, lookAt, cascades, resolution);

            var light = new Sunlight(shadowMap, color);
            this.Linker.AddComponent(entity, light);
        }
    }
}
