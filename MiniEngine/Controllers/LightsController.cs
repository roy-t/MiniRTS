using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;
using System.Collections.Generic;
using KeyboardInput = MiniEngine.Input.KeyboardInput;

namespace MiniEngine.Controllers
{
    public sealed class LightsController
    {
        private readonly EntityCreator EntityCreator;
        private readonly EntityController EntityController;

        private readonly LightsFactory LightsFactory;
        private readonly EntityLinker EntityLinker;
        private readonly List<Entity> TemporaryEntities;

        public LightsController(
            EntityCreator entityCreator,
            EntityController entityController,
            EntityLinker entityLinker,
            LightsFactory lightsFactory)
        {
            this.EntityCreator = entityCreator;
            this.EntityController = entityController;
            this.LightsFactory = lightsFactory;
            this.EntityLinker = entityLinker;
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
                this.EntityController.DestroyEntity(entity);
            }
            this.TemporaryEntities.Clear();
        }

        public void RemoveAllLights()
        {
            this.EntityLinker.RemoveComponents<PointLight>();
            this.EntityLinker.RemoveComponents<Sunlight>();
            this.EntityLinker.RemoveComponents<DirectionalLight>();
            this.EntityLinker.RemoveComponents<ShadowCastingLight>();
            this.EntityLinker.RemoveComponents<AmbientLight>();

            RemoveCreatedLights();
        }
        
        private Entity CreateTempEntity()
        {
            var entity = this.EntityCreator.CreateEntity();
            this.TemporaryEntities.Add(entity);

            return entity;
        }
    }
}
