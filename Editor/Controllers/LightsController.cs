using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Controllers
{
    public sealed class LightsController
    {
        private readonly EntityManager EntityManager;
        private readonly LightsFactory LightsFactory;
        private readonly IComponentContainer<PointLight> PointLightContainer;
        private readonly IComponentContainer<Sunlight> SunlightContainer;
        private readonly IComponentContainer<DirectionalLight> DirectionalLightContainer;
        private readonly IComponentContainer<ShadowCastingLight> ShadowCastingLightContainer;
        private readonly IComponentContainer<AmbientLight> AmbientLightContainer;

        private readonly List<Entity> TemporaryEntities;

        public LightsController(EntityManager entityManager, LightsFactory lightsFactory,
            IComponentContainer<PointLight> pointLightContainer,
            IComponentContainer<Sunlight> sunlightContainer,
            IComponentContainer<DirectionalLight> directionalLightContainer,
            IComponentContainer<ShadowCastingLight> shadowCastingLightContainer,
            IComponentContainer<AmbientLight> ambientLightContainer)
        {
            this.EntityManager = entityManager;
            this.LightsFactory = lightsFactory;
            this.PointLightContainer = pointLightContainer;
            this.SunlightContainer = sunlightContainer;
            this.DirectionalLightContainer = directionalLightContainer;
            this.ShadowCastingLightContainer = shadowCastingLightContainer;
            this.AmbientLightContainer = ambientLightContainer;
            this.TemporaryEntities = new List<Entity>();
        }

        public Entity CreatePointLight(Vector3 position)
        {
            var entity = this.CreateTempEntity();
            this.LightsFactory.PointLightFactory.Construct(entity, position, Color.White, 10.0f, 1.0f);
            return entity;
        }

        public Entity CreateSunLight(Vector3 position, Vector3 lookAt)
        {
            var entity = this.CreateTempEntity();
            this.LightsFactory.SunlightFactory.Construct(entity, Color.White, position, lookAt);
            return entity;
        }

        public Entity CreateDirectionalLight(Vector3 position, Vector3 lookAt)
        {
            var entity = this.CreateTempEntity();
            this.LightsFactory.DirectionalLightFactory.Construct(entity, lookAt - position, Color.White);
            return entity;
        }

        public Entity CreateShadowCastingLight(Vector3 position, Vector3 lookAt)
        {
            var entity = this.CreateTempEntity();
            this.LightsFactory.ShadowCastingLightFactory.Construct(entity, position, lookAt, Color.White);
            return entity;
        }

        public Entity CreateAmbientLight()
        {
            var entity = this.CreateTempEntity();
            this.LightsFactory.AmbientLightFactory.Construct(entity, Color.White);
            return entity;
        }

        public void RemoveCreatedLights()
        {
            foreach(var entity in this.TemporaryEntities)
            {
                this.EntityManager.Controller.DestroyEntity(entity);
            }
            this.TemporaryEntities.Clear();
        }

        public void RemoveAllLights()
        {
            this.PointLightContainer.Clear();
            this.SunlightContainer.Clear();
            this.DirectionalLightContainer.Clear();
            this.ShadowCastingLightContainer.Clear();
            this.AmbientLightContainer.Clear();

            this.RemoveCreatedLights();
        }
        
        private Entity CreateTempEntity()
        {
            var entity = this.EntityManager.Creator.CreateEntity();
            this.TemporaryEntities.Add(entity);

            return entity;
        }
    }
}
