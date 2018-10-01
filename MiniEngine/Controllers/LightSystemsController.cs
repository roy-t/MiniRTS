using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;
using System.Collections.Generic;
using KeyboardInput = MiniEngine.Input.KeyboardInput;

namespace MiniEngine.Controllers
{
    public sealed class LightSystemsController
    {
        private readonly KeyboardInput KeyboardInput;
        private readonly PerspectiveCamera Camera;
        private readonly EntityController EntityController;

        private readonly PointLightSystem PointLightSystem;
        private readonly SunlightSystem SunlightSystem;
        private readonly DirectionalLightSystem DirectionalLightSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;        


        private readonly List<Entity> TemporaryEntities;        

        public LightSystemsController(
            KeyboardInput keyboardInput,
            PerspectiveCamera camera,
            EntityController entityController,
            PointLightSystem pointLightSystem,
            SunlightSystem sunlightSystem,
            DirectionalLightSystem directionalLightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.KeyboardInput = keyboardInput;
            this.Camera = camera;
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
                this.PointLightSystem.Add(CreateTempEntity(), this.Camera.Position, Color.White, 10.0f, 1.0f);
            }

            if (this.KeyboardInput.Click(Keys.S))
            {
                var exists = false;
                foreach (var entity in this.TemporaryEntities)
                {
                    if (this.SunlightSystem.Contains(entity))
                    {
                        this.SunlightSystem.Remove(entity);
                        this.SunlightSystem.Add(entity, Color.White, this.Camera.Position, this.Camera.LookAt);
                        exists = true;
                    }
                }

                if (!exists)
                {
                    this.SunlightSystem.RemoveAll();
                    this.SunlightSystem.Add(CreateTempEntity(), Color.White, this.Camera.Position, this.Camera.LookAt);
                }
            }

            if (this.KeyboardInput.Click(Keys.D))
            {
                this.DirectionalLightSystem.Add(CreateTempEntity(), Vector3.Normalize(this.Camera.LookAt - this.Camera.Position), Color.White * 0.5f);
            }

            if (this.KeyboardInput.Click(Keys.C))
            {
                this.ShadowCastingLightSystem.Add(CreateTempEntity(), this.Camera.Position, this.Camera.LookAt, Color.White);
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
            var entity = this.EntityController.CreateEntity();
            this.TemporaryEntities.Add(entity);

            return entity;
        }
    }
}
