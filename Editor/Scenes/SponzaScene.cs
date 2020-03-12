using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
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

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Sponza";

        private AModel carModel;

        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.carModel = this.SceneBuilder.BuildCar(new Pose(Vector3.Up * 6));

            this.SceneBuilder.BuildSponzaLit(new Pose(Vector3.Zero, 0.05f));
            this.SceneBuilder.BuildStainedGlass();
            this.SceneBuilder.BuildFirePlace();
            this.SceneBuilder.BuildBulletHoles();
            this.SceneBuilder.BuildCube(new Pose(new Vector3(20, 10, 0), 0.01f));
            this.SceneBuilder.BuildCube(new Pose(new Vector3(20, 20, 0), 0.01f));

            this.Skybox = this.SceneBuilder.SponzaSkybox;
        }

        public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation)
            => new Pose(translation, scale, rotY, rotX, rotZ);

        public void RenderUI() { }

        public void Update(Seconds elapsed)
        {
            (this.carModel.Animation as CarAnimation).Update(elapsed);
            return;
        }
    }
}
