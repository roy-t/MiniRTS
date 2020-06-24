using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;

        public SponzaScene(SceneBuilder sceneBuilder)
        {
            this.SceneBuilder = sceneBuilder;
        }

        public void LoadContent(Content content)
            => this.Skybox = content.SponzaSkybox;

        public string Name => "Sponza";

        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.SceneBuilder.BuildSponzaLit(Vector3.Zero, 0.05f);
            this.SceneBuilder.BuildStainedGlass();
            this.SceneBuilder.BuildFirePlace();
            this.SceneBuilder.BuildBulletHoles();
            this.SceneBuilder.BuildCube(new Vector3(20, 10, 0), 0.01f);
            this.SceneBuilder.BuildCube(new Vector3(20, 20, 0), 0.01f);
        }


        public void RenderUI() { }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
        }
    }
}
