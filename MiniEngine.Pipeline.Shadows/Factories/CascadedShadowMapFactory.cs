using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Shadows.Factories
{
    public sealed class CascadedShadowMapFactory : AComponentFactory
    {        
        private const int DefaultResolution = 1024;
        private static readonly float[] DefaultCascadeDistances =
        {
            0.075f,
            0.15f,
            0.3f,
            1.0f
        };

        public CascadedShadowMapFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker) { }

        public void Build(Entity entity, Vector3 position, Vector3 lookAt,
            int cascades, int resolution, float[] cascadeDistances)
        {
            var cascadedShadowMap = new ShadowMapCascades(this.Device, resolution, cascades, position, lookAt, cascadeDistances);
            this.Linker.AddComponent(entity, cascadedShadowMap);

            for (var i = 0; i < cascades; i++)
            {                
                var shadowMap = new ShadowMap(cascadedShadowMap.DepthMapArray, cascadedShadowMap.ColorMapArray, i, cascadedShadowMap.ShadowCameras[i]);
                this.Linker.AddComponent(entity, shadowMap);
            }     
        }

        public void Construct(Entity entity, Vector3 position, Vector3 lookAt, int cascades, int resolution = DefaultResolution)
            => this.Build(entity, position, lookAt, cascades, resolution, DefaultCascadeDistances);

        public void Deconstruct(Entity entity)
        {
            this.Linker.RemoveComponents<ShadowMapCascades>(entity);
            this.Linker.RemoveComponents<ShadowMap>(entity);            
        }
    }
}
