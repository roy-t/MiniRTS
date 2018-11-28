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
    public sealed class LightSystemsController
    {
        private readonly KeyboardInput KeyboardInput;
        private readonly PerspectiveCamera Camera;
        private readonly EntityCreator EntityCreator;
        private readonly EntityController EntityController;

        private readonly LightsFactory LightsFactory;
        private readonly EntityLinker EntityLinker;
        private readonly List<Entity> TemporaryEntities;

        public LightSystemsController(
            KeyboardInput keyboardInput,
            PerspectiveCamera camera,
            EntityCreator entityCreator,
            EntityController entityController,
            LightsFactory lightsFactory,
            EntityLinker entityLinker)
        {
            this.KeyboardInput = keyboardInput;
            this.Camera = camera;
            this.EntityCreator = entityCreator;
            this.EntityController = entityController;
            this.LightsFactory = lightsFactory;
            this.EntityLinker = entityLinker;
            this.TemporaryEntities = new List<Entity>();
        }

        public void Update(Seconds elapsed)
        {
            if (this.KeyboardInput.Click(Keys.P))
            {
                this.LightsFactory.PointLightFactory.Construct(this.CreateTempEntity(), this.Camera.Position, Color.White, 10.0f, 1.0f);
            }

            if (this.KeyboardInput.Click(Keys.S))
            {
                this.EntityLinker.RemoveComponents<Sunlight>();
                this.LightsFactory.SunlightFactory.Construct(this.CreateTempEntity(), Color.White, this.Camera.Position, this.Camera.LookAt);
            }

            if (this.KeyboardInput.Click(Keys.D))
            {
                this.LightsFactory.DirectionalLightFactory.Construct(this.CreateTempEntity(), Vector3.Normalize(this.Camera.LookAt - this.Camera.Position), Color.White * 0.5f);
            }

            if (this.KeyboardInput.Click(Keys.C))
            {
                this.LightsFactory.ShadowCastingLightFactory.Construct(this.CreateTempEntity(), this.Camera.Position, this.Camera.LookAt, Color.White);
            }

            if (this.KeyboardInput.Click(Keys.R))
            {
                foreach (var entity in this.TemporaryEntities)
                {
                    this.EntityController.DestroyEntity(entity);
                }

                this.TemporaryEntities.Clear();
            }
        }

        private Entity CreateTempEntity()
        {
            var entity = this.EntityCreator.CreateEntity();
            this.TemporaryEntities.Add(entity);

            return entity;
        }
    }
}
