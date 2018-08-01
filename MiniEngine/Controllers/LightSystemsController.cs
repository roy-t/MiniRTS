using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Input;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;
using System.Collections.Generic;

namespace MiniEngine.Controllers
{
    public sealed class LightSystemsController
    {
        private static readonly Seconds ActiveTimeout = 2.0f;

        private readonly KeyboardInput KeyboardInput;
        private readonly PerspectiveCamera Camera;
        private readonly EntityController EntityController;

        private readonly PointLightSystem PointLightSystem;
        private readonly SunlightSystem SunlightSystem;
        private readonly DirectionalLightSystem DirectionalLightSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;        


        private readonly List<Entity> TemporaryEntities;

        private Seconds lastInputTimer;
        private bool isActive;

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

            this.lastInputTimer = 0.0f;
            this.isActive = false;

            this.TemporaryEntities = new List<Entity>();
        }

        public bool Update(Seconds elapsed)
        {
            this.lastInputTimer += elapsed;
            
            if (!this.KeyboardInput.Hold(Keys.LeftControl))
            {
                return false;
            }

            if (!this.isActive)
            {
                if (this.KeyboardInput.Click(Keys.L))
                {
                    this.isActive = true;
                    this.lastInputTimer = 0.0f;
                    return true;
                }
            }
            else
            {
                if (this.lastInputTimer > ActiveTimeout)
                {
                    this.isActive = false;                    
                }

                if (HandleInput())
                {
                    this.isActive = false;
                    return true;
                }
            }

            return false;
        }

        private bool HandleInput()
        {
            if (this.KeyboardInput.Click(Keys.P))
            {
                this.PointLightSystem.Add(CreateTempEntity(), this.Camera.Position, Color.White, 10.0f, 1.0f);
                return true;
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
                                                
                return true;
            }

            if (this.KeyboardInput.Click(Keys.D))
            {
                this.DirectionalLightSystem.Add(CreateTempEntity(), Vector3.Normalize(this.Camera.LookAt - this.Camera.Position), Color.White);
                return true;
            }

            if (this.KeyboardInput.Click(Keys.C))
            {
                this.ShadowCastingLightSystem.Add(CreateTempEntity(), this.Camera.Position, this.Camera.LookAt, Color.White);
                return true;
            }

            if (this.KeyboardInput.Click(Keys.R))
            {
                foreach (var entity in this.TemporaryEntities)
                {
                    this.EntityController.DestroyEntity(entity);
                }

                this.TemporaryEntities.Clear();
                return true;
            }

            return false;
        }

        private Entity CreateTempEntity()
        {
            var entity = this.EntityController.CreateEntity();
            this.TemporaryEntities.Add(entity);

            return entity;
        }


        //// HACK: dropping some lights
        //var selectedScene = this.scenes[this.currentSceneIndex];
        //if (this.KeyboardInput.Click(Keys.Q))
        //{
        //    var color = ColorUtilities.PickRandomColor();                
        //    var lightEntity = this.systemCollection.CreateEntity();
        //    this.systemCollection.PointLightSystem.Add(
        //        lightEntity,
        //        this.perspectiveCamera.Position,
        //        color,
        //        10,
        //        1.0f);
        //}

        //if (this.KeyboardInput.Click(Keys.LeftAlt))
        //{                
        //    var lightEntity = this.systemCollection.CreateEntity();
        //    this.renderSystem.ShadowCastingLightSystem.Add(
        //        lightEntity,
        //        this.perspectiveCamera.Position,
        //        this.perspectiveCamera.LookAt,
        //        Color.White);
        //}

        //if (this.KeyboardInput.Click(Keys.H))
        //{
        //    //selectedScene.Sunlights.ForEach(x => x.Move(this.perspectiveCamera.Position, this.perspectiveCamera.LookAt));
        //}
    }
}
