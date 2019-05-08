using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Primitives;
using MiniEngine.Rendering;
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
        private readonly DynamicTextureFactory DynamicTextureFactory;
        private readonly ProjectorFactory ProjectorFactory;
        private readonly EntityLinker Linker;
        private readonly DebugInfoFactory debugInfoFactory;
        private readonly PipelineBuilder PipelineBuilder;

        public SponzaScene(
            EntityCreator entityCreator,
            SceneBuilder sceneBuilder,
            LightsFactory lightsFactory,
            OpaqueModelFactory opaqueModelFactory,
            TransparentModelFactory transparentModelFactory,
            DebugInfoFactory debugInfoFactory,
            AdditiveEmitterFactory emitterFactory,
            DynamicTextureFactory dynamicTextureFactory,
            ProjectorFactory projectorFactory,
            PipelineBuilder pipelineBuilder,
            EntityLinker linker)
        {
            this.EntityCreator = entityCreator;
            this.SceneBuilder = sceneBuilder;
            this.LightsFactory = lightsFactory;
            this.OpaqueModelFactory = opaqueModelFactory;
            this.TransparentModelFactory = transparentModelFactory;
            this.debugInfoFactory = debugInfoFactory;
            this.EmitterFactory = emitterFactory;
            this.DynamicTextureFactory = dynamicTextureFactory;
            this.ProjectorFactory = projectorFactory;
            this.PipelineBuilder = pipelineBuilder;
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

            var entity = this.EntityCreator.CreateEntity();
            var dynamicTexture = this.DynamicTextureFactory.Construct(entity, new Vector3(-60, 8, -25), new Vector3(-60, 8, 24), 1024, 1024, "Firewatcher");

            this.PipelineBuilder.AddAll(dynamicTexture.Pipeline);
            this.debugInfoFactory.Construct(entity);

            var entity2 = this.EntityCreator.CreateEntity();
            var projector = this.ProjectorFactory.Construct(entity2, dynamicTexture.FinalTarget, Color.White, new Vector3(-52, 8, 10), new Vector3(-60, 8, 250));
            this.debugInfoFactory.Construct(entity2);
        }

        public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation) 
            => new Pose(translation, scale, rotY, rotX, rotZ);

        public void Update(Seconds elapsed)
        {

        }
    }
}
