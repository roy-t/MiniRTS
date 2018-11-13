using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Pipeline.Lights.Systems;
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

        private readonly PointLightSystem PointLightSystem;
        private readonly SunlightSystem SunlightSystem;
        private readonly DirectionalLightSystem DirectionalLightSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;


        private readonly List<Entity> TemporaryEntities;

        public LightSystemsController(
            KeyboardInput keyboardInput,
            PerspectiveCamera camera,
            EntityCreator entityCreator,
            EntityController entityController,
            PointLightSystem pointLightSystem,
            SunlightSystem sunlightSystem,
            DirectionalLightSystem directionalLightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.KeyboardInput = keyboardInput;
            this.Camera = camera;
            this.EntityCreator = entityCreator;
            this.EntityController = entityController;
            this.PointLightSystem = pointLightSystem;
            this.SunlightSystem = sunlightSystem;
            this.DirectionalLightSystem = directionalLightSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;

            this.TemporaryEntities = new List<Entity>();
        }

        public void Update(Seconds elapsed)
        {
            if (this.KeyboardInput.Click(Keys.P))
            {
                this.PointLightSystem.Add(this.CreateTempEntity(), this.Camera.Position, Color.White, 10.0f, 1.0f);
            }

            if (this.KeyboardInput.Click(Keys.S))
            {
                this.SunlightSystem.RemoveAll();
                this.SunlightSystem.Add(this.CreateTempEntity(), Color.White, this.Camera.Position, this.Camera.LookAt);
            }

            if (this.KeyboardInput.Click(Keys.D))
            {
                this.DirectionalLightSystem.Add(this.CreateTempEntity(), Vector3.Normalize(this.Camera.LookAt - this.Camera.Position), Color.White * 0.5f);
            }

            if (this.KeyboardInput.Click(Keys.C))
            {
                this.ShadowCastingLightSystem.Add(this.CreateTempEntity(), this.Camera.Position, this.Camera.LookAt, Color.White);
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
