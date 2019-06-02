using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Primitives;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class DemoScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;

        public DemoScene(SceneBuilder sceneBuilder)
        {
            this.SceneBuilder = sceneBuilder;
        }

        public void LoadContent(ContentManager content) 
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Demo";

        public void Set()
        {
            this.SceneBuilder.BuildSponzaLit(new Pose(Vector3.Zero, 0.05f));
            this.SceneBuilder.BuildStainedGlass();
            this.SceneBuilder.BuildFirePlace();
            this.SceneBuilder.BuildBulletHoles();

            this.SceneBuilder.BuildCutScene();
        }

        public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation) 
            => new Pose(translation, scale, rotY, rotX, rotZ);

        public void Update(Seconds elapsed)
        {

        }
    }
}
