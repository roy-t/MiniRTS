using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Systems;
using System.Collections.Generic;

namespace MiniEngine.Controllers
{
    public sealed class LightsController
    {
        private readonly EntityManager EntityManager;
        private readonly LightsFactory LightsFactory;

        private readonly List<Entity> TemporaryEntities;

        public LightsController(EntityManager entityManager, LightsFactory lightsFactory)
        {
            this.EntityManager = entityManager;
            this.LightsFactory = lightsFactory;
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
            this.EntityManager.Linker.RemoveComponents<PointLight>();
            this.EntityManager.Linker.RemoveComponents<Sunlight>();
            this.EntityManager.Linker.RemoveComponents<DirectionalLight>();
            this.EntityManager.Linker.RemoveComponents<ShadowCastingLight>();
            this.EntityManager.Linker.RemoveComponents<AmbientLight>();

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
