using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Systems;

namespace MiniEngine.Rendering.Systems
{
    public sealed class ShadowMapSystem : ISystem
    {
        private const int DefaultResolution = 1024;

        private readonly GraphicsDevice Device;
        private readonly Effect ShadowMapEffect;
        private readonly ModelSystem ModelSystem;
        private readonly Dictionary<Entity, ShadowMap> ShadowMaps;

        public ShadowMapSystem(GraphicsDevice device, Effect shadowMapEffect, ModelSystem modelSystem)
        {
            this.Device = device;
            this.ShadowMapEffect = shadowMapEffect;
            this.ModelSystem = modelSystem;

            this.ShadowMaps = new Dictionary<Entity, ShadowMap>();
        }

        public void Add(Entity entity, IViewPoint viewPoint, int resolution = DefaultResolution)
        {
            this.ShadowMaps.Add(entity, new ShadowMap(this.Device, resolution, 1, viewPoint));
        }

        public void Add(Entity entity, IViewPoint[] viewPoints, int cascades, int resolution = DefaultResolution)
        {
            this.ShadowMaps.Add(entity, new ShadowMap(this.Device, resolution, cascades, viewPoints));
        }

        public ShadowMap Get(Entity entity) => this.ShadowMaps[entity];

        public bool Contains(Entity entity) => this.ShadowMaps.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var shadowMap = this.ShadowMaps[entity];            
            return $"shadow map, dimensions: {shadowMap.DepthMap.Width}x{shadowMap.DepthMap.Height}";
        }

        public void Remove(Entity entity) => this.ShadowMaps.Remove(entity);

        public void RenderShadowMaps()
        {
            using (this.Device.ShadowMapState())
            {
                foreach (var shadowMap in this.ShadowMaps.Values)
                {
                    for (var i = 0; i < shadowMap.Cascades; i++)
                    {
                        this.Device.SetRenderTarget(shadowMap.DepthMap, i);
                        this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);
                        this.ModelSystem.DrawModels(shadowMap.ViewPoints[i], this.ShadowMapEffect);
                    }                    
                }
            }
        }
    }
}
