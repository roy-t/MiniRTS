using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Primitives;
using MiniEngine.Systems;
namespace MiniEngine.Scenes
{
    public sealed class SceneBuilder
    {
        private readonly EntityManager EntityManager;
        private readonly LightsFactory LightsFactory;
        private readonly OpaqueModelFactory OpaqueModelFactory;
        private readonly TransparentModelFactory TransparentModelFactory;
        private readonly ProjectorFactory ProjectorFactory;
        private readonly AdditiveEmitterFactory AdditiveEmitterFactory;
        private readonly AveragedEmitterFactory AveragedEmitterFactory;
        private readonly OutlineFactory OutlineFactory;

        private Model sponza;
        private Model plane;
        private Model lizard;
        private Texture2D explosion;
        private Texture2D explosion2;
        private Texture2D smoke;
        private Texture2D bulletHole;

        public SceneBuilder(EntityManager entityManager,
            LightsFactory lightsFactory,
            OpaqueModelFactory opaqueModelFactory,
            TransparentModelFactory transparentModelFactory,            
            ProjectorFactory projectorFactory,
            AdditiveEmitterFactory additiveEmitterFactory,
            AveragedEmitterFactory averagedEmitterFactory,
            OutlineFactory outlineFactory)
        {
            this.EntityManager = entityManager;
            this.LightsFactory = lightsFactory;
            this.OpaqueModelFactory = opaqueModelFactory;
            this.TransparentModelFactory = transparentModelFactory;
            this.ProjectorFactory = projectorFactory;
            this.AdditiveEmitterFactory = additiveEmitterFactory;
            this.AveragedEmitterFactory = averagedEmitterFactory;
            this.OutlineFactory = outlineFactory;
        }

        public void LoadContent(ContentManager content)
        {
            this.sponza = content.Load<Model>(@"Scenes\Sponza\Sponza");
            this.plane = content.Load<Model>(@"Scenes\Sponza\Plane");
            this.lizard = content.Load<Model>(@"Scenes\Zima\Lizard\lizard");
            this.explosion = content.Load<Texture2D>(@"Particles\Explosion");
            this.explosion2 = content.Load<Texture2D>(@"Particles\Explosion2");
            this.smoke = content.Load<Texture2D>(@"Particles\Smoke");
            this.bulletHole = content.Load<Texture2D>(@"Decals\BulletHole");
        }

        public Entity BuildSponzaLit(Pose pose)
        {
            var entity = this.EntityManager.Creator.CreateEntity();
            
            this.LightsFactory.AmbientLightFactory.Construct(entity, Color.White * 0.5f);
            this.LightsFactory.SunlightFactory.Construct(entity, Color.White, Vector3.Up, (Vector3.Left * 0.75f) + (Vector3.Backward * 0.1f));
            this.OpaqueModelFactory.Construct(entity, this.sponza, pose);

            return entity;
        }

        public Entity BuildLizard(Pose pose)
        {
            var entity = this.EntityManager.Creator.CreateEntity();
            this.OpaqueModelFactory.Construct(entity, this.lizard, pose);
            //this.LightsFactory.PointLightFactory.Construct(entity, new Vector3(55, 8, 20), Color.White, 50.0f, 0.75f);

            return entity;
        }

        public Entity[] BuildStainedGlass()
        {
            var entities = this.EntityManager.Creator.CreateEntities(2);

            var position = new Vector3(-40.5f, 30.0f, 3.2f);
            var world = new Pose(position, 4.4f * 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0);
            //var world = CreateScaleRotationTranslation(4.4f * 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0, position);
            this.TransparentModelFactory.Construct(entities[0], this.plane, world);
            this.OutlineFactory.Construct(entities[0]);

            position = new Vector3(-40.5f, 30.0f, -7.2f);
            //world = CreateScaleRotationTranslation(4.4f * 0.01f, 0, MathHelper.PiOver4, 0, position);
            world = new Pose(position, 4.4f * 0.01f, MathHelper.PiOver4);
            this.TransparentModelFactory.Construct(entities[1], this.plane, world);
            this.OutlineFactory.Construct(entities[1]);

            return entities;
        }

        public Entity BuildFirePlace()
        {
            var entity = this.EntityManager.Creator.CreateEntity();

            var particleSpawn = new Vector3(-60.5f, 6.0f, 20.0f);

            this.AveragedEmitterFactory.ConstructAveragedEmitter(entity, particleSpawn, this.smoke, 1, 1, 2.0f);
            this.AdditiveEmitterFactory.ConstructAdditiveEmitter(entity, particleSpawn, this.explosion2, 1, 1, 1.0f);
            var emitter = this.AdditiveEmitterFactory.ConstructAdditiveEmitter(entity, particleSpawn, this.explosion, 8, 8, 0.075f);
            emitter.SpawnInterval = 0;
            emitter.Spread = 0.75f;
            emitter.TimeToLive = 2.25f;

            this.LightsFactory.PointLightFactory.Construct(entity, particleSpawn, Color.IndianRed, 20.0f, 1.0f);
            //var light = particleSpawn + (Vector3.Up * 3);
            //this.LightsFactory.ShadowCastingLightFactory.Construct(this.particleEntity, light, light + Vector3.Up + (Vector3.Left * 0.001f), Color.IndianRed);

            return entity;
        }

        public Entity BuildBulletHoles()
        {
            var entity = this.EntityManager.Creator.CreateEntity();

            var random = new Random(12345);

            var center = new Vector3(-71.2f, 10, -25);
            var forward = Vector3.Left;

            for(var i = 0; i < 110; i++)
            {
                var u = (float)(random.NextDouble() * 15) - 7.5f;
                var v = (float)(random.NextDouble() * 15) - 7.5f;

                var offset = new Vector3(0, u, v);
                var projector = this.ProjectorFactory.Construct(entity, this.bulletHole, Color.White, center + offset, center + offset + forward);
                projector.SetMaxDistance(1.0f);
            }

            

            return entity;
        }
    }
}
