using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : IScene
    {
        private readonly EntityController EntityController;
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly SunlightSystem SunlightSystem;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;
        private readonly ModelSystem ModelSystem;

        private Entity worldEntity;
        private Entity planeEntity;
        private Model sponza;
        private Model plane;

        public SponzaScene(
            EntityController entityController,
            AmbientLightSystem ambientLightSystem,
            SunlightSystem sunlightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem,
            ModelSystem modelSystem)
        {
            this.EntityController = entityController;
            this.AmbientLightSystem = ambientLightSystem;
            this.SunlightSystem = sunlightSystem;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
            this.ModelSystem = modelSystem;
        }

        public void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Scenes\Sponza\Sponza");
            this.plane = content.Load<Model>(@"Scenes\Sponza\Plane");
        }

        public void Set()
        {
            this.worldEntity = this.EntityController.CreateEntity();
            
            this.AmbientLightSystem.Add(this.worldEntity, Color.White * 0.25f);
            this.SunlightSystem.Add(this.worldEntity, Color.White, Vector3.Up, Vector3.Right * 0.75f + Vector3.Forward * 0.1f);
            
            this.ModelSystem.Add(this.worldEntity, this.sponza, Matrix.CreateScale(0.05f));

            this.planeEntity = this.EntityController.CreateEntity();

            var position = new Vector3(-50.5f, 30.0f, 3.2f);
            var offset = new Vector3(1.0f, 0.25f, 0.0f) * 8;

            this.ShadowCastingLightSystem.Add(this.planeEntity, position + offset, position - offset, Color.White);
            
            var world = Matrix.Identity
                * Matrix.CreateScale(4.4f * 0.01f)
                * Matrix.CreateRotationY(MathHelper.PiOver2)                
                * Matrix.CreateTranslation(position)
                ;
            this.ModelSystem.Add(this.planeEntity, this.plane, world, ModelType.Transparent);            
        }

        public void Update(Seconds elapsed)
        {
            
        }
    }
}
