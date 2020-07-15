using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MiniEngine.Pipeline.Utilities;

namespace MiniEngine.GameLogic
{
    public sealed class Content
    {
        private readonly SkyboxBuilder SkyboxBuilder;

        public Content(GraphicsDevice device)
        {
            this.SkyboxBuilder = new SkyboxBuilder(device);
        }


        public Model Exhaust { get; private set; }

        public Model Fighter { get; private set; }
        public Model Sponza { get; private set; }
        public Model Plane { get; private set; }
        public Model Cube { get; private set; }
        public Model Gear { get; private set; }
        public Texture2D Explosion { get; private set; }
        public Texture2D Explosion2 { get; private set; }
        public Texture2D Smoke { get; private set; }
        public Texture2D BulletHole { get; private set; }
        public Texture2D Mask { get; private set; }
        public Song Song { get; private set; }

        public TextureCube NullSkybox { get; private set; }

        public TextureCube SponzaSkybox { get; private set; }

        public void LoadContent(ContentManager content)
        {
            this.Exhaust = content.Load<Model>(@"Scenes\RocketParts\Exhaust");
            this.Fighter = content.Load<Model>(@"Scenes\Primitives\fighter");
            this.Sponza = content.Load<Model>(@"Scenes\Sponza\Sponza");
            this.Cube = content.Load<Model>(@"Scenes\Primitives\Cube");
            this.Gear = content.Load<Model>(@"Scenes\Primitives\Gear");
            this.Plane = content.Load<Model>(@"Scenes\Sponza\Plane");
            this.Explosion = content.Load<Texture2D>(@"Particles\Explosion");
            this.Explosion2 = content.Load<Texture2D>(@"Particles\Explosion2");
            this.Smoke = content.Load<Texture2D>(@"Particles\Smoke");
            this.BulletHole = content.Load<Texture2D>(@"Decals\BulletHole");
            this.Mask = content.Load<Texture2D>(@"StarMask");
            this.Song = content.Load<Song>(@"Music\Zemdens");

            this.NullSkybox = this.SkyboxBuilder.BuildSkyBox(Color.Black);

            var back = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\back");
            var down = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\down");
            var front = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\front");
            var left = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\left");
            var right = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\right");
            var up = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\up");

            this.SponzaSkybox = this.SkyboxBuilder.BuildSkyBox(back, down, front, left, right, up);
        }

    }
}
