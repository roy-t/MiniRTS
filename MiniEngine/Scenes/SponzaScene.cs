using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : IScene
    {
        private Entity worldEntity;
        private Model sponza;                

        public void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Sponza\Sponza");            
        }

        public void Set(SystemCollection systems)
        {
            this.worldEntity = systems.CreateEntity();
            
            systems.AmbientLightSystem.Add(this.worldEntity, Color.White * 0.25f);
            systems.SunlightSystem.Add(this.worldEntity, Color.White, Vector3.Up, Vector3.Right * 0.75f + Vector3.Forward * 0.1f);
            systems.ModelSystem.Add(this.worldEntity, this.sponza, Matrix.CreateScale(0.05f));
        }

        public void Update(Seconds elapsed)
        {
            
        }
    }
}
