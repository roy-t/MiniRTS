using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Systems;

namespace MiniEngine.Controllers
{
    public sealed class LightsController
    {
        private readonly EntityController EntityController;
        private readonly PoseFactory PoseFactory;
        private readonly LightsFactory LightsFactory;

        private readonly List<Entity> Entities;

        public LightsController(EntityController entityController,
            PoseFactory poseFactory,
            LightsFactory lightsFactory)
        {
            this.EntityController = entityController;
            this.PoseFactory = poseFactory;
            this.LightsFactory = lightsFactory;
            this.Entities = new List<Entity>();
        }

        public Entity CreatePointLight(Vector3 position)
        {
            var entity = this.CreateEntityWithPose(position);
            this.LightsFactory.PointLightFactory.Construct(entity, Color.White, 10.0f, 1.0f);
            return entity;
        }

        public Entity CreateSunLight(Vector3 position, Vector3 lookAt)
        {
            var entity = this.CreateEntityWithPose(position);
            this.LightsFactory.SunlightFactory.Construct(entity, Color.White, position, lookAt);
            return entity;
        }

        public Entity CreateDirectionalLight(Vector3 position, Vector3 lookAt)
        {
            var entity = this.CreateEntityWithPose(position);
            this.LightsFactory.DirectionalLightFactory.Construct(entity, lookAt - position, Color.White);
            return entity;
        }

        public Entity CreateShadowCastingLight(Vector3 position, Vector3 lookAt)
        {
            var entity = this.CreateEntityWithPose(position);
            this.LightsFactory.ShadowCastingLightFactory.Construct(entity, position, lookAt, Color.White);
            return entity;
        }

        public Entity CreateAmbientLight()
        {
            var entity = this.EntityController.CreateEntity();
            this.Entities.Add(entity);
            this.LightsFactory.AmbientLightFactory.Construct(entity, Color.White);
            return entity;
        }

        public void RemoveCreatedLights()
        {
            foreach (var entity in this.Entities)
            {
                this.EntityController.DestroyEntity(entity);
            }
            this.Entities.Clear();
        }

        private Entity CreateEntityWithPose(Vector3 position)
        {
            var entity = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity, position);
            this.Entities.Add(entity);

            return entity;
        }
    }
}
