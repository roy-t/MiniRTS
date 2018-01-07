using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Units;
using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : AScene
    {
        private Model sponza;

        public SponzaScene(GraphicsDevice device, Camera camera)
            : base(device, camera)
        {
            this.DirectionalLights.Add(new DirectionalLight(Vector3.Normalize(Vector3.Forward + Vector3.Down), Color.White * 0.75f));
            this.DirectionalLights.Add(new DirectionalLight(Vector3.Normalize(Vector3.Forward + Vector3.Up + Vector3.Left), Color.BlueViolet * 0.25f));
            this.AmbientLight = Color.White * 0.1f;
        }

        public override void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Sponza\Sponza");
        }

        public override void Update(Seconds elapsed)
        {
            
        }

        public override void Draw()
        {
            using (this.Device.GeometryState())
            {
                DrawModel(this.sponza, Matrix.CreateScale(0.05f));
            }
        }
    }
}
