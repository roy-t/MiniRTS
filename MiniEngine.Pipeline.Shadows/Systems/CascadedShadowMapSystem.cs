using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Shadows.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using System;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Shadows.Systems
{
    public sealed class CascadedShadowMapSystem : ISystem
    {
        private const int DefaultResolution = 1024;

        private readonly GraphicsDevice Device;
        private readonly EntityCreator EntityCreator;
        private readonly Dictionary<Entity, CascadedShadowMap> CascadedShadowMaps;
        private readonly ShadowMapSystem ShadowMapSystem;


        public CascadedShadowMapSystem(GraphicsDevice device, EntityCreator entityCreator, ShadowMapSystem shadowMapSystem)
        {
            this.Device = device;
            this.EntityCreator = entityCreator;
            this.CascadedShadowMaps = new Dictionary<Entity, CascadedShadowMap>();
            this.ShadowMapSystem = shadowMapSystem;
        }

        public void Add(Entity entity, int cascades, Reference<IViewPoint>[] viewPoints, int resolution = DefaultResolution)
        {
            var cascadedShadowMap = new CascadedShadowMap(this.Device, resolution, cascades, viewPoints);
            var childEntities = this.EntityCreator.CreateChildEntities(entity, cascades);
            for(var i = 0; i < cascades; i++)
            {
                this.ShadowMapSystem.Add(childEntities[i], cascadedShadowMap.DepthMapArray, cascadedShadowMap.ColorMapArray, i, viewPoints[i]);
            }

            this.CascadedShadowMaps.Add(entity, cascadedShadowMap);
        }

        public CascadedShadowMap Get(Entity entity) => this.CascadedShadowMaps[entity];

        public bool Contains(Entity entity) => this.CascadedShadowMaps.ContainsKey(entity);
        public string Describe(Entity entity) => throw new NotImplementedException();
        public void Remove(Entity entity)
        {
            this.CascadedShadowMaps.Remove(entity);
            var children = this.EntityCreator.GetChilderen(entity);
            foreach(var child in children)
            {
                this.ShadowMapSystem.Remove(child);
            }
        }
    }
}
