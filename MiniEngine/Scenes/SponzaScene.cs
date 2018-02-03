using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Units;
using MiniEngine.Utilities;
using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : AScene
    {
        private Model sponza;
        
        public SponzaScene(GraphicsDevice device, Camera camera)
            : base(device, camera)
        {
            this.AmbientLight = Color.White * 0.25f;

            this.DirectionalLights.Add(new DirectionalLight(Vector3.Normalize(new Vector3(0.25f, -1.0f, 0.65f)), Color.White * 0.2f));
            this.PointLights.Add(new PointLight(new Vector3(55, 10, 22), Color.White, 10, 1));
            

            
        }

        public PointLight PointLight { get; private set; }

        public override void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Sponza\Sponza");
        }

        public override void Update(Seconds elapsed)
        {
            
        }

        public override void Draw()
        {
            DrawModel(this.sponza, Matrix.CreateScale(0.05f));
        }

        public void NewLight(Vector3 position)
        {
            var color = ColorUtilities.PickRandomColor();
            this.PointLight = new PointLight(position, color, 10, 1.0f);
            this.PointLights.Add(this.PointLight);
        }
    }
}
