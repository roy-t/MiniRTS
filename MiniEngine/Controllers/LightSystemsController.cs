using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Input;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Controllers
{
    public sealed class LightSystemsController
    {
        private static readonly Seconds ActiveTimeout = 2.0f;

        private readonly KeyboardInput KeyboardInput;
        private readonly PerspectiveCamera Camera;
        private readonly SystemCollection Systems;

        private readonly List<Entity> TemporaryEntities;
        private readonly Entity RootEntity;

        private Seconds lastInputTimer;
        private bool isActive;
      
        public LightSystemsController(KeyboardInput keyboardInput, PerspectiveCamera camera, SystemCollection systems)
        {
            this.KeyboardInput = keyboardInput;
            this.Camera = camera;
            this.Systems = systems;

            this.lastInputTimer = 0.0f;
            this.isActive = false;

            this.TemporaryEntities = new List<Entity>();
            this.RootEntity = systems.CreateEntity();            
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
                this.Systems.PointLightSystem.Add(CreateTempEntity(), this.Camera.Position, Color.White, 10.0f, 1.0f);
                return true;
            }

            if (this.KeyboardInput.Click(Keys.S))
            {
                this.Systems.SunlightSystem.Remove(this.RootEntity);
                this.Systems.SunlightSystem.Add(this.RootEntity, Color.White, this.Camera.Position, this.Camera.LookAt);
                return true;
            }

            if (this.KeyboardInput.Click(Keys.D))
            {
                this.Systems.DirectionalLightSystem.Add(CreateTempEntity(), Vector3.Normalize(this.Camera.LookAt - this.Camera.Position), Color.White);
                return true;
            }

            if (this.KeyboardInput.Click(Keys.C))
            {
                this.Systems.ShadowCastingLightSystem.Add(CreateTempEntity(), this.Camera.Position, this.Camera.LookAt, Color.White);
                return true;
            }

            if (this.KeyboardInput.Click(Keys.R))
            {
                foreach (var entity in this.TemporaryEntities.Union(
                    new[]
                    {
                        this.RootEntity
                    }))
                {
                    this.Systems.PointLightSystem.Remove(entity);
                    this.Systems.SunlightSystem.Remove(entity);
                    this.Systems.DirectionalLightSystem.Remove(entity);
                    this.Systems.ShadowCastingLightSystem.Remove(entity);
                }

                this.TemporaryEntities.Clear();
                return true;
            }

            return false;
        }

        private Entity CreateTempEntity()
        {
            var entity = this.Systems.CreateEntity();
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
