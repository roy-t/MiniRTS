using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Shadows.Factories
{
    public sealed class CascadedShadowMapFactory : AComponentFactory<CascadedShadowMap>
    {
        private const int DefaultResolution = 1024;

        private static readonly float[] DefaultCascadeDistances =
        {
            0.075f,
            0.15f,
            0.3f,
            1.0f
        };

        private readonly ShadowMapFactory ShadowMapFactory;

        public CascadedShadowMapFactory(GraphicsDevice device, ShadowMapFactory shadowMapFactory, EntityLinker linker)
            : base(device, linker)
        {
            this.ShadowMapFactory = shadowMapFactory;
        }

        public CascadedShadowMap Construct(Entity entity, Vector3 position, Vector3 lookAt,
            int cascades, int resolution, float[] cascadeDistances)
        {
            var cascadedShadowMap = new CascadedShadowMap(this.Device, resolution, cascades, position, lookAt, cascadeDistances);
            this.Linker.AddComponent(entity, cascadedShadowMap);

            for (var i = 0; i < cascades; i++)
            {
                this.ShadowMapFactory.Construct(entity, cascadedShadowMap.DepthMapArray, cascadedShadowMap.ColorMapArray, i, cascadedShadowMap.ShadowCameras[i]);
            }

            return cascadedShadowMap;
        }

        public CascadedShadowMap Construct(Entity entity, Vector3 position, Vector3 lookAt, int cascades, int resolution = DefaultResolution)
            => this.Construct(entity, position, lookAt, cascades, resolution, DefaultCascadeDistances);

        public override void Deconstruct(Entity entity)
        {
            this.Linker.RemoveComponents<CascadedShadowMap>(entity);
            this.Linker.RemoveComponents<ShadowMap>(entity);
        }
    }
}
