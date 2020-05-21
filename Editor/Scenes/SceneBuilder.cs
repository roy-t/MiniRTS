using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MiniEngine.CutScene;
using MiniEngine.GameLogic;
using MiniEngine.GameLogic.Factories;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Pipeline.Debug.Factories;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Pipeline.Particles.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
using MiniEngine.Rendering;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Factories;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class SceneBuilder
    {
        private readonly EntityController EntityController;
        private readonly SkyboxBuilder SkyboxBuilder;

        private readonly LightsFactory LightsFactory;
        private readonly OpaqueModelFactory OpaqueModelFactory;
        private readonly TransparentModelFactory TransparentModelFactory;
        private readonly CarAnimationFactory CarAnimationFactory;
        private readonly UVAnimationFactory UVAnimationFactory;
        private readonly ProjectorFactory ProjectorFactory;
        private readonly AdditiveEmitterFactory AdditiveEmitterFactory;
        private readonly AveragedEmitterFactory AveragedEmitterFactory;
        private readonly DynamicTextureFactory DynamicTextureFactory;
        private readonly DebugInfoFactory DebugInfoFactory;
        private readonly DebugLineFactory DebugLineFactory;
        private readonly WaypointFactory WaypointFactory;
        private readonly PipelineBuilder PipelineBuilder;
        private readonly PoseFactory PoseFactory;
        private readonly OffsetFactory OffsetFactory;
        private readonly ParentFactory ParentFactory;

        private Model terrain;
        private Model car;
        private Model tank;
        private Model fighter;
        private Model sponza;
        private Model plane;
        private Model lizard;
        private Model cube;
        private Model gear;
        private Texture2D explosion;
        private Texture2D explosion2;
        private Texture2D smoke;
        private Texture2D bulletHole;
        private Texture2D mask;
        private Song song;

        public SceneBuilder(EntityController entityController,
            SkyboxBuilder skyboxBuilder,
            LightsFactory lightsFactory,
            OpaqueModelFactory opaqueModelFactory,
            TransparentModelFactory transparentModelFactory,
            CarAnimationFactory carAnimationFactory,
            UVAnimationFactory uvAnimationFactory,
            ProjectorFactory projectorFactory,
            AdditiveEmitterFactory additiveEmitterFactory,
            AveragedEmitterFactory averagedEmitterFactory,
            DynamicTextureFactory dynamicTextureFactory,
            DebugInfoFactory debugInfoFactory,
            DebugLineFactory debugLineFactory,
            WaypointFactory waypointFactory,
            PipelineBuilder pipelineBuilder,
            OffsetFactory offsetFactory,
            PoseFactory poseFactory,
            ParentFactory parentFactory)
        {
            this.EntityController = entityController;
            this.SkyboxBuilder = skyboxBuilder;
            this.LightsFactory = lightsFactory;
            this.OpaqueModelFactory = opaqueModelFactory;
            this.TransparentModelFactory = transparentModelFactory;
            this.CarAnimationFactory = carAnimationFactory;
            this.UVAnimationFactory = uvAnimationFactory;
            this.ProjectorFactory = projectorFactory;
            this.AdditiveEmitterFactory = additiveEmitterFactory;
            this.AveragedEmitterFactory = averagedEmitterFactory;
            this.DynamicTextureFactory = dynamicTextureFactory;
            this.DebugInfoFactory = debugInfoFactory;
            this.DebugLineFactory = debugLineFactory;
            this.WaypointFactory = waypointFactory;
            this.PipelineBuilder = pipelineBuilder;
            this.PoseFactory = poseFactory;
            this.OffsetFactory = offsetFactory;
            this.ParentFactory = parentFactory;
        }

        public void LoadContent(ContentManager content)
        {
            this.terrain = content.Load<Model>(@"Scenes\Primitives\plane3");
            this.car = content.Load<Model>(@"Scenes\Primitives\car_textured");
            this.tank = content.Load<Model>(@"Scenes\Primitives\tank");
            this.fighter = content.Load<Model>(@"Scenes\Primitives\fighter");
            this.sponza = content.Load<Model>(@"Scenes\Sponza\Sponza");
            this.cube = content.Load<Model>(@"Scenes\Primitives\Cube");
            this.gear = content.Load<Model>(@"Scenes\Primitives\Gear");
            this.plane = content.Load<Model>(@"Scenes\Sponza\Plane");
            this.lizard = content.Load<Model>(@"Scenes\Zima\Lizard\lizard");
            this.explosion = content.Load<Texture2D>(@"Particles\Explosion");
            this.explosion2 = content.Load<Texture2D>(@"Particles\Explosion2");
            this.smoke = content.Load<Texture2D>(@"Particles\Smoke");
            this.bulletHole = content.Load<Texture2D>(@"Decals\BulletHole");
            this.mask = content.Load<Texture2D>(@"StarMask");
            this.song = content.Load<Song>(@"Music\Zemdens");

            this.NullSkybox = this.SkyboxBuilder.BuildSkyBox(Color.Black);

            var back = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\back");
            var down = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\down");
            var front = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\front");
            var left = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\left");
            var right = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\right");
            var up = content.Load<Texture2D>(@"Scenes\Sponza\Skybox\up");

            this.SponzaSkybox = this.SkyboxBuilder.BuildSkyBox(back, down, front, left, right, up);
        }


        public Song LoadMusic() => this.song;

        public TextureCube NullSkybox { get; private set; }

        public TextureCube SponzaSkybox { get; private set; }

        public DebugLine CreateDebugLine(IReadOnlyList<Vector3> linePositions, Color color)
        {
            var entity = this.EntityController.CreateEntity();
            return this.DebugLineFactory.Construct(entity, linePositions, color);
        }

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

        public OpaqueModel BuildSponza(Vector3 position, float scale)
        {
            var entity = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity, position, scale);
            var (model, bounds) = this.OpaqueModelFactory.Construct(entity, this.sponza);
            return model;
        }

        public Entity BuildLizard(Vector3 position, float scale)
        {
            var entity = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity, position, scale);
            this.OpaqueModelFactory.Construct(entity, this.lizard);
            //this.LightsFactory.PointLightFactory.Construct(entity, new Vector3(55, 8, 20), Color.White, 50.0f, 0.75f);

            return entity;
        }

        public (Pose, OpaqueModel, Bounds) BuildCube(Vector3 position, float scale)
        {
            var entity = this.EntityController.CreateEntity();
            var pose = this.PoseFactory.Construct(entity, position, scale);
            var (model, bounds) = this.OpaqueModelFactory.Construct(entity, this.cube);

            return (pose, model, bounds);
        }

        public ShadowCastingLight BuildLionSpotLight()
        {
            var entity = this.EntityController.CreateEntity();
            return this.LightsFactory.ShadowCastingLightFactory.Construct(entity, new Vector3(40, 13, 27), new Vector3(53, 11, 12), Color.White, 2048);
        }

        public void BuildSponzaLit(Vector3 position, float scale)
        {
            this.BuildSponzaAmbientLight();
            this.BuildSponzeSunLight();
            this.BuildSponza(position, scale);
        }

        public void BuildGear(Vector3 position, float scale)
        {
            var entity = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity, position, scale);
            var model = this.OpaqueModelFactory.Construct(entity, this.gear);
            //this.DebugInfoFactory.Construct(entity, model);
        }

        public Car BuildCar(Vector3 position, float scale)
        {
            var entity = this.EntityController.CreateEntity();
            var pose = this.PoseFactory.Construct(entity, position, scale);
            var (model, bounds) = this.OpaqueModelFactory.Construct(entity, this.car);
            var animation = this.CarAnimationFactory.Construct(entity, model.Model.Tag as SkinningData);

            //this.DebugInfoFactory.Construct(entity, model);

            return new Car(model, pose, animation);
        }

        public (Pose, OpaqueModel, Bounds, UVAnimation) BuildTank(Vector3 position, float scale)
        {
            var entity = this.EntityController.CreateEntity();
            var pose = this.PoseFactory.Construct(entity, position, scale);
            var (model, bounds) = this.OpaqueModelFactory.Construct(entity, this.tank);
            this.DebugInfoFactory.Construct(entity, IconType.Model);

            var animation = this.UVAnimationFactory.Construct(entity, "TRACK_LEFT", "TRACK_RIGHT");

            return (pose, model, bounds, animation);
        }


        public (Pose, OpaqueModel, Bounds) BuildFighter(Vector3 position, float scale)
        {
            var entity = this.EntityController.CreateEntity();
            var pose = this.PoseFactory.Construct(entity, position, scale);
            var (model, bounds) = this.OpaqueModelFactory.Construct(entity, this.fighter);
            this.DebugInfoFactory.Construct(entity, IconType.Model);

            return (pose, model, bounds);
        }

        public void BuildTerrain(Vector2 size)
        {
            var entity = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity, Vector3.Zero, 1.0f);
            var (model, bounds) = this.OpaqueModelFactory.Construct(entity, this.terrain);

            model.TextureScale = size;
        }


        public Entity[] BuildStainedGlass()
        {
            var entities = this.EntityController.CreateEntities(2);

            var position = new Vector3(-40.5f, 30.0f, 3.2f);
            this.PoseFactory.Construct(entities[0], position, 4.4f * 0.01f, MathHelper.PiOver2, MathHelper.PiOver2, 0);
            var model1 = this.TransparentModelFactory.Construct(entities[0], this.plane);
            //this.DebugInfoFactory.Construct(entities[0], model1);

            position = new Vector3(-40.5f, 30.0f, -7.2f);
            this.PoseFactory.Construct(entities[1], position, 4.4f * 0.01f, MathHelper.PiOver4);
            var model2 = this.TransparentModelFactory.Construct(entities[1], this.plane);
            //this.DebugInfoFactory.Construct(entities[1], model2);

            return entities;
        }

        public PointLight BuildFirePlace()
        {
            var entity = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity, new Vector3(-60.5f, 6.0f, 20.0f), Vector3.One * 2, 0, MathHelper.PiOver2, 0);
            this.AveragedEmitterFactory.ConstructAveragedEmitter(entity, this.smoke, 1, 1);

            var entity2 = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity2, new Vector3(-60.5f, 6.0f, 20.0f), Vector3.One, 0, MathHelper.PiOver2, 0);
            this.AdditiveEmitterFactory.ConstructAdditiveEmitter(entity2, this.explosion2, 1, 1);

            var entity3 = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity3, new Vector3(-60.5f, 6.0f, 20.0f), Vector3.One * 0.075f, 0, MathHelper.PiOver2, 0);
            var emitter = this.AdditiveEmitterFactory.ConstructAdditiveEmitter(entity3, this.explosion, 8, 8);
            emitter.SpawnInterval = 0;
            emitter.Spread = 0.75f;
            emitter.TimeToLive = 2.25f;


            var pointLight = this.LightsFactory.PointLightFactory.Construct(entity, Color.IndianRed, 20.0f, 1.0f);

            var cameraPosition = new Vector3(-60.5f, 8.0f, 20.0f);
            var projectorPosition = new Vector3(-60.5f, 0.0f, 20.0f);
            var lookAt = cameraPosition + (new Vector3(0.001f, 1, 0) * 10);

            var lightEntity = this.EntityController.CreateEntity();
            var dynamicTexture = this.DynamicTextureFactory.Construct(lightEntity, cameraPosition, lookAt, 1024, 1024, this.NullSkybox, "Firewatcher");
            this.PipelineBuilder.AddParticlePipeline(dynamicTexture.Pipeline);

            var color = Color.White * 0.2f;
            var projector = this.ProjectorFactory.Construct(lightEntity, dynamicTexture.FinalTarget, this.mask, color, projectorPosition, lookAt);
            projector.SetMinDistance(10.0f);
            projector.SetMaxDistance(30.0f);

            return pointLight;
        }

        public Parent BuildParent(string name)
        {
            var parentEntity = this.EntityController.CreateEntity(name);
            var parent = this.ParentFactory.Construct(parentEntity);
            return parent;
        }

        public (AdditiveEmitter, Pose, Offset) BuildRCS(Entity target, Vector3 offset, float yaw, float pitch, float roll)
        {
            var entity = this.EntityController.CreateEntity();
            var pose = this.PoseFactory.Construct(entity, offset, 0.2f);
            var offsetC = this.OffsetFactory.Construct(entity, offset, yaw, pitch, roll, target);

            var emitter = this.AdditiveEmitterFactory.ConstructAdditiveEmitter(entity, this.explosion2, 1, 1);
            emitter.SpawnInterval = 0;
            emitter.Spread = 0.05f;
            emitter.Speed = 5.0f;
            emitter.TimeToLive = 0.1f;

            return (emitter, pose, offsetC);
        }

        public Entity BuildBulletHoles()
        {
            var entity = this.EntityController.CreateEntity();

            var random = new Random(12345);

            var center = new Vector3(-71.2f, 10, -25);
            var forward = Vector3.Left;

            for (var i = 0; i < 1 /*100*/; i++)
            {
                var u = (float)(random.NextDouble() * 15) - 7.5f;
                var v = (float)(random.NextDouble() * 15) - 7.5f;

                var offset = new Vector3(0, u, v);
                var projector = this.ProjectorFactory.Construct(entity, this.bulletHole, Color.White, center + offset, center + offset + forward);
                projector.SetMaxDistance(1.0f);
            }

            return entity;
        }

        public void BuildCutScene()
        {
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

                var entity = this.EntityController.CreateEntity();
                this.WaypointFactory.Construct(entity, speed, position, lookAt);
            }
        }
    }
}
