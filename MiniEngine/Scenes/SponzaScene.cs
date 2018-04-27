using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Mathematics;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Units;
using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : AScene
    {
        private readonly PerspectiveCamera PerspectiveCamera;
        private readonly Matrix World = Matrix.CreateScale(0.05f);
        private Model sponza;
        
        public SponzaScene(GraphicsDevice device, PerspectiveCamera perspectiveCamera)
            : base(device)
        {
            this.PerspectiveCamera = perspectiveCamera;
            this.AmbientLight = Color.White * 0.25f;

            //this.DirectionalLights.Add(new DirectionalLight(Vector3.Normalize(new Vector3(0.25f, -1.0f, 0.65f)), Color.White * 0.2f));
            //this.PointLights.Add(new PointLight(new Vector3(55, 10, 22), Color.White, 10, 1));           
            //this.ShadowCastingLights.Add(new ShadowCastingLight(device, new Vector3(29.5f, 35.7f, 0.3f), new Vector3(35, 35, 0), Color.White));
        }              

        public override void LoadContent(ContentManager content, GraphicsDevice device)
        {
            this.sponza = content.Load<Model>(@"Sponza\Sponza");

            var boundingBox = this.sponza.ComputeBoundingBox(this.World);
            var boundingSphere = this.sponza.ComputeBoundingSphere(this.World);
                        
            this.Sunlights.Add(new Sunlight(device));
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
