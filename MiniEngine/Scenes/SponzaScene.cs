using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Units;
using MiniEngine.Utilities.Extensions;
using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : AScene
    {        
        private readonly Matrix World = Matrix.CreateScale(0.05f);
        private Model sponza;

        private int worldEntity;

        public SponzaScene()
        {
            this.AmbientLight = Color.White * 0.25f;

            //this.DirectionalLights.Add(new DirectionalLight(Vector3.Normalize(new Vector3(0.25f, -1.0f, 0.65f)), Color.White * 0.2f));
            //this.PointLights.Add(new PointLight(new Vector3(55, 10, 22), Color.White, 10, 1));           
            //this.ShadowCastingLights.Add(new ShadowCastingLight(device, new Vector3(29.5f, 35.7f, 0.3f), new Vector3(35, 35, 0), Color.White));
        }

        public override void LoadContent(ContentManager content, SystemCollection systems)
        {
            this.sponza = content.Load<Model>(@"Sponza\Sponza");

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
