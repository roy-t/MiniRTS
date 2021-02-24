using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.ParticipatingMedia;
using MiniEngine.Graphics.Particles;
using MiniEngine.Graphics.Particles.Functions;
using MiniEngine.Graphics.Transparency;
using MiniEngine.Graphics.Volumes;
using MiniEngine.SceneManagement;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SponzaScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly SkyboxSceneService Skybox;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly GeneratedAssets Assets;

        public SponzaScene(GraphicsDevice device, SkyboxSceneService skybox, EntityAdministrator entities, ComponentAdministrator components, GeneratedAssets assets)
        {
            this.Device = device;
            this.Skybox = skybox;
            this.Entities = entities;
            this.Components = components;
            this.Assets = assets;
        }

        public void RenderMainMenuItems()
           => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            var sponza = content.Load<GeometryModel>("sponza/sponza");
            this.CreateModel(sponza, Matrix.CreateScale(0.05f));

            var entity = this.Entities.Create();

            // Add dust
            var cube = CubeGenerator.Generate(this.Device);
            this.Components.Add(ParticipatingMediaComponent.Create(entity, this.Device, cube, this.Device.Viewport.Width, this.Device.Viewport.Height, 4.0f, new Color(0.1f, 0.1f, 0.1f)));
            this.Components.Add(new TransformComponent(entity, Matrix.CreateScale(200, 150.0f, 120.0f)));

            AdditiveParticles(content);
            TransparentParticles(content);

            AddVolume(content);
        }

        private void AddVolume(ContentStack content)
        {
            var entity = this.Entities.Create();

            var transform = new TransformComponent(entity, new Vector3(25, 10, 0), Vector3.One);
            this.Components.Add(transform);

            var albedo = content.Load<Texture2D>("Particles/muzzle_01");

            var material = new Material(albedo, this.Assets.NormalPixel(), this.Assets.PlasticPixel, this.Assets.RoughnessPixel(0.5f), this.Assets.VisiblePixel);
            var volume = new VolumeComponent(entity, material);
            this.Components.Add(volume);
        }

        private void AdditiveParticles(ContentStack content)
        {
            var particle = content.Load<Texture2D>("Textures/AdditiveParticle");
            var particleEntity = this.Entities.Create();
            this.Components.Add(new TransformComponent(particleEntity, Matrix.CreateTranslation(Vector3.Left * 5) * Matrix.CreateRotationX(MathHelper.PiOver2)));
            var spawn = new IntervalSpawnFunction();
            var update = new LinearUpdateFunction();

            var component = new ParticleFountainComponent(particleEntity, particle.GraphicsDevice);
            component.AddEmitter(particle, spawn, update);

            this.Components.Add(component);
        }

        private void TransparentParticles(ContentStack content)
        {
            var particle = content.Load<Texture2D>("Textures/TransparentParticle");
            var particleEntity = this.Entities.Create();

            this.Components.Add(new TransformComponent(particleEntity, new Vector3(-44.0f, 0.0f, 0.0f), Vector3.One, 0.0f, MathHelper.PiOver2));
            var spawn = new IntervalSpawnFunction() { SpawnInterval = 0.4f };
            var update = new LinearUpdateFunction();

            var component = new TransparentParticleFountainComponent(particleEntity, particle.GraphicsDevice);
            component.AddEmitter(particle, spawn, update);

            this.Components.Add(component);
        }

        private void CreateModel(GeometryModel model, Matrix transform)
        {
            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, model));
            this.Components.Add(new TransformComponent(entity, transform));
        }
    }
}
