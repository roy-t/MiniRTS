using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class SponzaScene : IScene
    {
        private readonly EntityCreator EntityCreator;
        private readonly SceneBuilder SceneBuilder;
        private readonly LightsFactory LightsFactory;
        private readonly OpaqueModelFactory OpaqueModelFactory;
        private readonly TransparentModelFactory TransparentModelFactory;
        private readonly AdditiveEmitterFactory EmitterFactory;
        private readonly EntityLinker Linker;
        private readonly DebugInfoFactory OutlineFactory;  

        public SponzaScene(
            EntityCreator entityCreator,
            SceneBuilder sceneBuilder,
            LightsFactory lightsFactory,
            OpaqueModelFactory opaqueModelFactory,
            TransparentModelFactory transparentModelFactory,
            DebugInfoFactory outlineFactory,
            AdditiveEmitterFactory emitterFactory,
            EntityLinker linker)
        {
            this.EntityCreator = entityCreator;
            this.SceneBuilder = sceneBuilder;
            this.LightsFactory = lightsFactory;
            this.OpaqueModelFactory = opaqueModelFactory;
            this.TransparentModelFactory = transparentModelFactory;
            this.OutlineFactory = outlineFactory;
            this.EmitterFactory = emitterFactory;
            this.Linker = linker;
        }

        public void LoadContent(ContentManager content) 
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Sponza";

        public void Set()
        {
            this.SceneBuilder.BuildSponzaLit(new Pose(Vector3.Zero, 0.05f));
            this.SceneBuilder.BuildStainedGlass();
            this.SceneBuilder.BuildFirePlace();
            this.SceneBuilder.BuildBulletHoles();
        }

        public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation) 
            => new Pose(translation, scale, rotY, rotX, rotZ);

        public void Update(Seconds elapsed)
        {

        }
    }
}
