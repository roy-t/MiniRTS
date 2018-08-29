using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly Effect ColorMapEffect;
        private readonly ModelSystem ModelSystem;
        private readonly Dictionary<Entity, ShadowMap> ShadowMaps;

        public ShadowMapSystem(
            GraphicsDevice device,
            Effect shadowMapEffect,
            Effect colorMapEffect,
            ModelSystem modelSystem)
        {
            this.Device = device;
            this.ShadowMapEffect = shadowMapEffect;
            this.ColorMapEffect = colorMapEffect;
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

        public ShadowMap DebugFoo() => this.ShadowMaps.Values.First();

        public bool Contains(Entity entity) => this.ShadowMaps.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var shadowMap = this.ShadowMaps[entity];
            return $"shadow map, dimensions: {shadowMap.DepthMap.Width}x{shadowMap.DepthMap.Height}";
        }

        public void Remove(Entity entity) => this.ShadowMaps.Remove(entity);

        public void RenderShadowMaps()
        {
            foreach (var shadowMap in this.ShadowMaps.Values)
            {
                for (var i = 0; i < shadowMap.Cascades; i++)
                {

                    var batches = this.ModelSystem.ComputeBatches(shadowMap.ViewPoints[i]);

                    this.Device.SetRenderTarget(shadowMap.DepthMap, i);
                    this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                    using (this.Device.ShadowMapState())
                    {
                        batches.DrawOpaque(this.ShadowMapEffect);                        
                    }

                    // Use the same depth buffer to generate the color buffer
                    this.Device.SetRenderTarget(shadowMap.ColorMap, i);
                    this.Device.Clear(ClearOptions.Target, Color.White, 1.0f, 0);

                    using (this.Device.ColorMapState())
                    {
                        batches.DrawTransparent(this.ColorMapEffect);
                    }
                }
            }
        }
    }
}
