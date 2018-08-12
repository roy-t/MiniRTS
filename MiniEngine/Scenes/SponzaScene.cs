using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : IScene
    {
        private readonly EntityController EntityController;
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly SunlightSystem SunlightSystem;
        private readonly ModelSystem ModelSystem;

        private Entity worldEntity;
        private Model sponza;

        public SponzaScene(
            EntityController entityController,
            AmbientLightSystem ambientLightSystem,
            SunlightSystem sunlightSystem,
            ModelSystem modelSystem)
        {
            this.EntityController = entityController;
            this.AmbientLightSystem = ambientLightSystem;
            this.SunlightSystem = sunlightSystem;
            this.ModelSystem = modelSystem;
        }

        public void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Scenes\Sponza\Sponza");            
        }

        public void Set()
        {
            this.worldEntity = this.EntityController.CreateEntity();
            
            this.AmbientLightSystem.Add(this.worldEntity, Color.White * 0.25f);
            this.SunlightSystem.Add(this.worldEntity, Color.White, Vector3.Up, Vector3.Right * 0.75f + Vector3.Forward * 0.1f);
            this.ModelSystem.Add(this.worldEntity, this.sponza, Matrix.CreateScale(0.05f));
        }

        public void Update(Seconds elapsed)
        {
            
        }
    }
}
