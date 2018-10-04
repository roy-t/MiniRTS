using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;
using MiniEngine.Utilities.Extensions;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : IScene
    {
        private readonly EntityController EntityController;
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly SunlightSystem SunlightSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;
        private readonly ModelSystem ModelSystem;
        private readonly ParticleSystem ParticleSystem;
        private readonly DebugRenderSystem DebugRenderSystem;

        private Entity worldEntity;
        private Entity planeEntity;
        private Entity planeEntity2;
        private Entity particleEntity;
        private Entity particleEntity2;
        private Entity particleEntity3;
        private Model sponza;
        private Model plane;
        private Texture2D explosion;
        private Texture2D explosion2;
        private Texture2D smoke;

        public SponzaScene(
            EntityController entityController,
            AmbientLightSystem ambientLightSystem,
            SunlightSystem sunlightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem,
            ModelSystem modelSystem,
            DebugRenderSystem debugRenderSystem,
            ParticleSystem particleSystem)
        {
            this.EntityController = entityController;
            this.AmbientLightSystem = ambientLightSystem;
            this.SunlightSystem = sunlightSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
            this.ModelSystem = modelSystem;
            this.DebugRenderSystem = debugRenderSystem;
            this.ParticleSystem = particleSystem;
        }

        public void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Scenes\Sponza\Sponza");
            this.plane = content.Load<Model>(@"Scenes\Sponza\Plane");
            this.explosion = content.Load<Texture2D>(@"Particles\Explosion");
            this.explosion2 = content.Load<Texture2D>(@"Particles\Explosion2");
            this.smoke = content.Load<Texture2D>(@"Particles\Smoke");
        }

        public void Set()
        {
            this.worldEntity = this.EntityController.CreateEntity();
            
            this.AmbientLightSystem.Add(this.worldEntity, Color.White * 0.25f);
            //this.SunlightSystem.Add(this.worldEntity, Color.White, Vector3.Up, Vector3.Right * 0.75f + Vector3.Forward * 0.1f);
            this.SunlightSystem.Add(this.worldEntity, Color.White, Vector3.Up, Vector3.Left * 0.75f + Vector3.Backward * 0.1f);

            this.ModelSystem.Add(this.worldEntity, this.sponza, Matrix.CreateScale(0.05f));
            

            this.planeEntity = this.EntityController.CreateEntity();

            var position = new Vector3(-40.5f, 30.0f, 3.2f);
            var offset = new Vector3(1.0f, 0.25f, 0.0f) * 8;

            //this.ShadowCastingLightSystem.Add(this.planeEntity, position + offset, position - offset, Color.White);

            var world = MatrixExtensions.CreateScaleRotationTranslation(4.4f * 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0, position);
            this.ModelSystem.Add(this.planeEntity, this.plane, world, ModelType.Transparent);            

            this.planeEntity2 = this.EntityController.CreateEntity();

            position = new Vector3(-40.5f, 30.0f, -7.2f);                        
            var world2 = MatrixExtensions.CreateScaleRotationTranslation(4.4f * 0.01f, 0, MathHelper.PiOver4, 0, position);

            this.ModelSystem.Add(this.planeEntity2, this.plane, world2, ModelType.Transparent);


            this.particleEntity = this.EntityController.CreateEntity();
            this.ParticleSystem.Add(this.particleEntity, new Vector3(-50.0f, 10.0f, 0.0f), this.smoke, 1, 1);

            this.particleEntity2 = this.EntityController.CreateEntity();
            this.ParticleSystem.Add(this.particleEntity2, new Vector3(-50.0f, 10.0f, 0.0f), this.explosion, 8, 8);

            this.particleEntity3 = this.EntityController.CreateEntity();
            this.ParticleSystem.Add(this.particleEntity3, new Vector3(-50.0f, 10.0f, 0.0f), this.explosion2, 1, 1);


            //this.DebugRenderSystem.Add(this.worldEntity, this.sponza, Matrix.CreateScale(0.05f));
            this.DebugRenderSystem.Add(this.planeEntity, this.plane, world);
            this.DebugRenderSystem.Add(this.planeEntity2, this.plane, world2);
        }

        public void Update(Seconds elapsed)
        {
            
        }
    }
}
