using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class SSAOScene : IScene
    {
        private readonly EntityCreator EntityCreator;
        private readonly LightsFactory LightsFactory;
        private readonly OpaqueModelFactory OpaqueModelFactory;
        private readonly TransparentModelFactory TransparentModelFactory;
        private readonly EmitterFactory EmitterFactory;
        private readonly EntityLinker Linker;
        private readonly OutlineFactory OutlineFactory;

        private Entity worldEntity;
        private Entity pointLightEntity;
        private Model sponza;
        private Model plane;
        private Texture2D explosion;
        private Texture2D explosion2;
        private Texture2D smoke;        

        public SSAOScene(
            EntityCreator entityCreator,
            LightsFactory lightsFactory,
            OpaqueModelFactory opaqueModelFactory,
            TransparentModelFactory transparentModelFactory,
            OutlineFactory outlineFactory,
            EmitterFactory emitterFactory,
            EntityLinker linker)
        {
            this.EntityCreator = entityCreator;
            this.LightsFactory = lightsFactory;
            this.OpaqueModelFactory = opaqueModelFactory;
            this.TransparentModelFactory = transparentModelFactory;
            this.OutlineFactory = outlineFactory;
            this.EmitterFactory = emitterFactory;
            this.Linker = linker;
        }

        public void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Scenes\Sponza\Sponza");
            this.plane = content.Load<Model>(@"Scenes\Sponza\Plane");
            this.explosion = content.Load<Texture2D>(@"Particles\Explosion");
            this.explosion2 = content.Load<Texture2D>(@"Particles\Explosion2");
            this.smoke = content.Load<Texture2D>(@"Particles\Smoke");
        }

        public string Name => "SSAO - Sponza";

        public void Set()
        {
            this.worldEntity = this.EntityCreator.CreateEntity();
            this.LightsFactory.AmbientLightFactory.Construct(this.worldEntity, Color.White * 0.25f);
            this.OpaqueModelFactory.Construct(this.worldEntity, this.sponza, CreateScaleRotationTranslation(0.05f, 0, 0, 0, Vector3.Zero));

            this.pointLightEntity = this.EntityCreator.CreateEntity();
            var position = new Vector3(50, 10, -3);
            this.LightsFactory.PointLightFactory.Construct(this.pointLightEntity, position, Color.White, 20.0f, 1.0f);            
        }

        public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation) 
            => new Pose(translation, scale, rotY, rotX, rotZ);

        public void Update(Seconds elapsed)
        {

        }
    }
}
