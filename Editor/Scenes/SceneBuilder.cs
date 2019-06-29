using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MiniEngine.CutScene;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Primitives;
using MiniEngine.Rendering;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class SceneBuilder
    {
        private readonly EntityController EntityController;
        private readonly LightsFactory LightsFactory;
        private readonly OpaqueModelFactory OpaqueModelFactory;
        private readonly TransparentModelFactory TransparentModelFactory;
        private readonly ProjectorFactory ProjectorFactory;
        private readonly AdditiveEmitterFactory AdditiveEmitterFactory;
        private readonly AveragedEmitterFactory AveragedEmitterFactory;
        private readonly DynamicTextureFactory DynamicTextureFactory;
        private readonly DebugInfoFactory DebugInfoFactory;
        private readonly WaypointFactory WaypointFactory;
        private readonly PipelineBuilder PipelineBuilder;

        private Model sponza;
        private Model plane;
        private Model lizard;
        private Texture2D explosion;
        private Texture2D explosion2;
        private Texture2D smoke;
        private Texture2D bulletHole;
        private Texture2D mask;
        private Song song;

        public SceneBuilder(EntityController entityController,
            LightsFactory lightsFactory,
            OpaqueModelFactory opaqueModelFactory,
            TransparentModelFactory transparentModelFactory,            
            ProjectorFactory projectorFactory,
            AdditiveEmitterFactory additiveEmitterFactory,
            AveragedEmitterFactory averagedEmitterFactory,
            DynamicTextureFactory dynamicTextureFactory,
            DebugInfoFactory debugInfoFactory,
            WaypointFactory waypointFactory,
            PipelineBuilder pipelineBuilder)
        {
            this.EntityController = entityController;
            this.LightsFactory = lightsFactory;
            this.OpaqueModelFactory = opaqueModelFactory;
            this.TransparentModelFactory = transparentModelFactory;
            this.ProjectorFactory = projectorFactory;
            this.AdditiveEmitterFactory = additiveEmitterFactory;
            this.AveragedEmitterFactory = averagedEmitterFactory;
            this.DynamicTextureFactory = dynamicTextureFactory;
            this.DebugInfoFactory = debugInfoFactory;
            this.WaypointFactory = waypointFactory;
            this.PipelineBuilder = pipelineBuilder;
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
            this.mask = content.Load<Texture2D>(@"StarMask");
            this.song = content.Load<Song>(@"Music\Zemdens");
        }

        public Song LoadMusic() => this.song;


        public AmbientLight BuildSponzaAmbientLight()
        {
            var entity = this.EntityController.CreateEntity();
            return this.LightsFactory.AmbientLightFactory.Construct(entity, Color.White * 0.5f);
        }

        public Sunlight BuildSponzeSunLight()
        {
            var entity = this.EntityController.CreateEntity();
            return this.LightsFactory.SunlightFactory.Construct(entity, Color.White, Vector3.Up, (Vector3.Left * 0.75f) + (Vector3.Backward * 0.1f));
        }
        
        public OpaqueModel BuildSponza(Pose pose)
        {
            var entity = this.EntityController.CreateEntity();
            return this.OpaqueModelFactory.Construct(entity, this.sponza, pose);
        }

        public Entity BuildLizard(Pose pose)
        {
            var entity = this.EntityController.CreateEntity();
            this.OpaqueModelFactory.Construct(entity, this.lizard, pose);
            //this.LightsFactory.PointLightFactory.Construct(entity, new Vector3(55, 8, 20), Color.White, 50.0f, 0.75f);

            return entity;
        }     
        
        public ShadowCastingLight BuildLionSpotLight()
        {
            var entity = this.EntityController.CreateEntity();
            return this.LightsFactory.ShadowCastingLightFactory.Construct(entity, new Vector3(40, 13, 27), new Vector3(53, 11, 12), Color.White, 2048);
        }

        public PointLight[] CreateFestiveLights()
        {
            var entity = this.EntityController.CreateEntity();
            var count = 20;
            var lights = new PointLight[count];

            var start = new Vector3(60, 30, 20);
            var end = new Vector3(-60, 30, 20);

            var distance = Vector3.Distance(start, end);
            var direction = Vector3.Normalize(end - start);
            var step = distance / count;

            for (var i = 0; i < count; i++)
            {
                var position = start + (direction * step * i);
                lights[i] = this.LightsFactory.PointLightFactory.Construct(entity, position, Color.White, 10.0f, 1.0f);
            }

            return lights;
        }

        public void BuildSponzaLit(Pose pose)
        {
            BuildSponzaAmbientLight();
            BuildSponzeSunLight();
            BuildSponza(pose);
        }


        public Entity[] BuildStainedGlass()
        {
            var entities = this.EntityController.CreateEntities(2);

            var position = new Vector3(-40.5f, 30.0f, 3.2f);
            var world = new Pose(position, 4.4f * 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0);
            this.TransparentModelFactory.Construct(entities[0], this.plane, world);
            this.DebugInfoFactory.Construct(entities[0]);

            position = new Vector3(-40.5f, 30.0f, -7.2f);
            world = new Pose(position, 4.4f * 0.01f, MathHelper.PiOver4);
            this.TransparentModelFactory.Construct(entities[1], this.plane, world);
            this.DebugInfoFactory.Construct(entities[1]);            

            return entities;
        }

        public PointLight BuildFirePlace()
        {
            var entity = this.EntityController.CreateEntity();

            var particleSpawn = new Vector3(-60.5f, 6.0f, 20.0f);

            this.AveragedEmitterFactory.ConstructAveragedEmitter(entity, particleSpawn, this.smoke, 1, 1, 2.0f);
            this.AdditiveEmitterFactory.ConstructAdditiveEmitter(entity, particleSpawn, this.explosion2, 1, 1, 1.0f);
            var emitter = this.AdditiveEmitterFactory.ConstructAdditiveEmitter(entity, particleSpawn, this.explosion, 8, 8, 0.075f);
            emitter.SpawnInterval = 0;
            emitter.Spread = 0.75f;
            emitter.TimeToLive = 2.25f;

            var pointLight = this.LightsFactory.PointLightFactory.Construct(entity, particleSpawn, Color.IndianRed, 20.0f, 1.0f);
            //var light = particleSpawn + (Vector3.Up * 3);
            //this.LightsFactory.ShadowCastingLightFactory.Construct(this.particleEntity, light, light + Vector3.Up + (Vector3.Left * 0.001f), Color.IndianRed);


            var cameraPosition = new Vector3(-60.5f, 8.0f, 20.0f);
            var projectorPosition = new Vector3(-60.5f, 0.0f, 20.0f);
            var lookAt = cameraPosition + (new Vector3(0.001f, 1, 0) * 10);

            var lightEntity = this.EntityController.CreateEntity();
            var dynamicTexture = this.DynamicTextureFactory.Construct(lightEntity, cameraPosition,  lookAt, 1024, 1024, "Firewatcher");                        
            this.PipelineBuilder.AddParticlePipeline(dynamicTexture.Pipeline);
            this.DebugInfoFactory.Construct(lightEntity);

            var color = Color.White * 0.2f;            
            var projector = this.ProjectorFactory.Construct(lightEntity, dynamicTexture.FinalTarget, this.mask, color, projectorPosition, lookAt);
            projector.SetMinDistance(10.0f);
            projector.SetMaxDistance(30.0f);

            return pointLight;
        }

        public Entity BuildBulletHoles()
        {
            var entity = this.EntityController.CreateEntity();

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

        public Entity BuildCutScene()
        {
            var entity = this.EntityController.CreateEntity();
            this.DebugInfoFactory.Construct(entity);

            var speeds = new MetersPerSecond[]
            {
                new MetersPerSecond(15.0f),
                new MetersPerSecond(15.0f),
                new MetersPerSecond(15.0f),
                new MetersPerSecond(6.0f),
                new MetersPerSecond(15.0f),
                new MetersPerSecond(15.0f),
                new MetersPerSecond(15.0f),
                new MetersPerSecond(15.0f),
                new MetersPerSecond(5.0f),
                new MetersPerSecond(5.0f)
            };

            var positions = new Vector3[]
            {
                new Vector3(60, 10, 20), // start position
                new Vector3(-60, 10, 20), // near fireplace
                new Vector3(-50, 15, 0), // side stepping column
                new Vector3(-10, 40, 0),  // center stage
                new Vector3(-30, 13, -10), // inspect windows
                new Vector3(-25, 34, -10), // side step to upper row
                new Vector3(-10, 34, -10), // in upper row
                new Vector3(20, 25, -7), // in upper row
                new Vector3(49, 10, -7), // pass lion
                new Vector3(49, 10, 20), // start position
            };

            var lookAts = new Vector3[]
            {                
                new Vector3(-60, 10, 20),
                new Vector3(-60, 0, 20),
                new Vector3(-60, 30, 20),
                new Vector3(-40, 0, 20),
                new Vector3(-50, 30, 10),
                new Vector3(-10, 40, 20),
                new Vector3(60, 40, 20),
                new Vector3(60, 10, 0),
                new Vector3(60, 10, 0),
                new Vector3(60, 10, 0)
            };

            for (var i = 0; i < positions.Length; i++)
            {
                var speed = speeds[i];
                var position = positions[i];
                var lookAt = lookAts[i];
                
                this.WaypointFactory.Construct(entity, speed, position, lookAt);
            }

            return entity;
        }    
    }
}
