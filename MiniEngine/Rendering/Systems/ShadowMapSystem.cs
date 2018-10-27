﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Effects;
using MiniEngine.Systems;

namespace MiniEngine.Rendering.Systems
{
    public sealed class ShadowMapSystem : ISystem
    {
        private const int DefaultResolution = 1024;

        private readonly GraphicsDevice Device;
        private readonly ModelSystem ModelSystem;
        private readonly ParticleSystem ParticleSystem;
        private readonly Dictionary<Entity, ShadowMap> ShadowMaps;

        public ShadowMapSystem(
            GraphicsDevice device,
            ModelSystem modelSystem,
            ParticleSystem particleSystem)
        {
            this.Device = device;
            this.ModelSystem = modelSystem;
            this.ParticleSystem = particleSystem;

            this.ShadowMaps = new Dictionary<Entity, ShadowMap>();
        }

        public bool Contains(Entity entity) => this.ShadowMaps.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var shadowMap = this.ShadowMaps[entity];
            return $"shadow map, dimensions: {shadowMap.DepthMap.Width}x{shadowMap.DepthMap.Height}";
        }

        public void Remove(Entity entity) => this.ShadowMaps.Remove(entity);

        public void Add(Entity entity, IViewPoint viewPoint, int resolution = DefaultResolution) => this.ShadowMaps.Add(entity, new ShadowMap(this.Device, resolution, 1, viewPoint));

        public void Add(Entity entity, IViewPoint[] viewPoints, int cascades, int resolution = DefaultResolution) => this.ShadowMaps.Add(entity, new ShadowMap(this.Device, resolution, cascades, viewPoints));

        public ShadowMap Get(Entity entity) => this.ShadowMaps[entity];

        public void RenderShadowMaps()
        {
            foreach (var shadowMap in this.ShadowMaps.Values)
            {
                for (var i = 0; i < shadowMap.Cascades; i++)
                {
                    var modelBatchList = this.ModelSystem.ComputeBatches(shadowMap.ViewPoints[i]);
                    var particleBatchList = this.ParticleSystem.ComputeBatches(shadowMap.ViewPoints[i]);

                    // First compute the shadow maps
                    this.Device.SetRenderTarget(shadowMap.DepthMap, i);
                    this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                    using (this.Device.ShadowMapState())
                    {
                        modelBatchList.OpaqueBatch.Draw(Techniques.ShadowMap);
                    }

                    // Read the depth buffer and render objects that are partially
                    // occluding, like a stained glass window
                    this.Device.SetRenderTarget(shadowMap.ColorMap, i);
                    this.Device.Clear(ClearOptions.Target, Color.White, 1.0f, 0);

                    using (this.Device.AlphaBlendOccluderState())
                    {
                        foreach (var batch in modelBatchList.TransparentBatches)
                        {
                            batch.Draw(Techniques.Textured);
                        }
                    }

                    // Read the depth buffer and render occluding particles
                    using (this.Device.AdditiveBlendOccluderState())
                    {
                        foreach (var batch in particleBatchList.Batches)
                        {
                            batch.Draw(Techniques.GrayScale);
                        }
                    }

                    // TODO: if a particle is behind a stained glass window
                    // it will shadow as if its in front of it. 
                }
            }
        }
    }
}