using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Lights.Systems;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Pipeline.Systems;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class ZScene : IScene
    {
        private readonly EntityCreator EntityCreator;
        private readonly LightsFactory LightsFactory;
        private readonly OpaqueModelFactory OpaqueModelFactory;
        private readonly TransparentModelFactory TransparentModelFactory;
        private readonly EmitterFactory EmitterFactory;
        private readonly DebugRenderSystem DebugRenderSystem;

        private Entity worldEntity;
        private Entity planeEntity;
        private Entity planeEntity2;
        private Entity particleEntity;
        private Entity particleEntity2;
        private Entity particleEntity3;
        private Model sponza;
        private Model plane;
        private Texture2D explosion;
        private Texture2D explosion2;
        private Texture2D smoke;        

        public ZScene(
            EntityCreator entityCreator,
            LightsFactory lightsFactory,
            SunlightSystem sunlightSystem,
            ShadowCastingLightSystem shadowCastingLightSystem,
            PointLightSystem pointLightSystem,
            OpaqueModelFactory opaqueModelFactory,
            TransparentModelFactory transparentModelFactory,
            DebugRenderSystem debugRenderSystem,
            EmitterFactory emitterFactory)
        {
            this.EntityCreator = entityCreator;
            this.LightsFactory = lightsFactory;
            this.OpaqueModelFactory = opaqueModelFactory;
            this.TransparentModelFactory = transparentModelFactory;
            this.DebugRenderSystem = debugRenderSystem;
            this.EmitterFactory = emitterFactory;
        }

        public void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Scenes\Sponza\Sponza");
            this.plane = content.Load<Model>(@"Scenes\Sponza\Plane");
            this.explosion = content.Load<Texture2D>(@"Particles\Explosion");
            this.explosion2 = content.Load<Texture2D>(@"Particles\Explosion2");
            this.smoke = content.Load<Texture2D>(@"Particles\Smoke");
        }

        public string Name => "Z-Scene";

        public void Set()
        {
            this.worldEntity = this.EntityCreator.CreateEntity();

            this.LightsFactory.AmbientLightFactory.Construct(this.worldEntity, Color.Purple * 0.5f);

            this.LightsFactory.SunlightFactory.Construct(this.worldEntity, Color.White, Vector3.Up, (Vector3.Left * 0.75f) + (Vector3.Backward * 0.1f));

            this.OpaqueModelFactory.Construct(this.worldEntity, this.sponza, CreateScaleRotationTranslation(0.05f, 0, 0, 0, Vector3.Zero));

            this.planeEntity = this.EntityCreator.CreateEntity();

            var position = new Vector3(-40.5f, 30.0f, 3.2f);

            var world = CreateScaleRotationTranslation(4.4f * 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0, position);
            this.TransparentModelFactory.Construct(this.planeEntity, this.plane, world);

            this.planeEntity2 = this.EntityCreator.CreateEntity();

            position = new Vector3(-40.5f, 30.0f, -7.2f);
            var world2 = CreateScaleRotationTranslation(4.4f * 0.01f, 0, MathHelper.PiOver4, 0, position);

            this.TransparentModelFactory.Construct(this.planeEntity2, this.plane, world2);

            var particleSpawn = new Vector3(-60.5f, 6.0f, 20.0f);
            this.particleEntity = this.EntityCreator.CreateEntity();
            this.EmitterFactory.Construct(this.particleEntity, particleSpawn, this.smoke, 1, 1);

            this.particleEntity2 = this.EntityCreator.CreateEntity();
            this.EmitterFactory.Construct(this.particleEntity2, particleSpawn, this.explosion, 8, 8);

            this.particleEntity3 = this.EntityCreator.CreateEntity();
            this.EmitterFactory.Construct(this.particleEntity3, particleSpawn, this.explosion2, 1, 1);

            this.LightsFactory.PointLightFactory.Construct(this.particleEntity, particleSpawn, Color.IndianRed, 20.0f, 1.0f);
            var light = particleSpawn + (Vector3.Up * 3);
            this.LightsFactory.ShadowCastingLightFactory.Construct(this.particleEntity, light, light + Vector3.Up + (Vector3.Left * 0.001f), Color.IndianRed);


            //this.DebugRenderSystem.Add(this.worldEntity, this.sponza, Matrix.CreateScale(0.05f));
            this.DebugRenderSystem.Add(this.planeEntity, this.plane, world.Matrix);
            this.DebugRenderSystem.Add(this.planeEntity2, this.plane, world2.Matrix);
        }

          public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation) 
            => new Pose(translation, scale, rotY, rotX, rotZ);

        public void Update(Seconds elapsed)
        {

        }
    }
}
