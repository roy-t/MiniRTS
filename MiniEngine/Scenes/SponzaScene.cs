using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : AScene
    {        
        private readonly Matrix World = Matrix.CreateScale(0.05f);
        private Model sponza;

        private Entity worldEntity;

        public SponzaScene()
        {
            this.AmbientLight = Color.White * 0.25f;
        }

        public override void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Sponza\Sponza");            
        }

        public override void Set(SystemCollection systems)
        {
            this.worldEntity = systems.CreateEntity();
            systems.SunlightSystem.Add(this.worldEntity, Color.White, Vector3.Up, Vector3.Right * 0.75f + Vector3.Forward * 0.1f);
        }

        public override void Update(Seconds elapsed)
        {
            
        }

        public override void Draw(IViewPoint viewPoint)
        {
            DrawModel(this.sponza, this.World, viewPoint);
        }

        public override void Draw(Effect effectOverride, IViewPoint viewPoint)
        {
            DrawModel(effectOverride, this.sponza, this.World, viewPoint);
        }

        
    }
}
