using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Primitives;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class LizardScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;

        public LizardScene(SceneBuilder sceneBuilder)
        {
            this.SceneBuilder = sceneBuilder;
        }

        public void LoadContent(ContentManager content) 
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Lizard";

        public void Set()
        {
            this.SceneBuilder.BuildSponzaLit(new Pose(Vector3.Zero, 0.05f, 0, MathHelper.Pi));
            this.SceneBuilder.BuildLizard(new Pose(new Vector3(0, 10, 0), 1));            
        }

        public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation) 
            => new Pose(translation, scale, rotY, rotX, rotZ);

        public void Update(Seconds elapsed)
        {

        }
    }
}
