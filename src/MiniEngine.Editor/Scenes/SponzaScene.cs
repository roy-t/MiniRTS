using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Mutators;
using MiniEngine.Graphics.Mutators.Functions;
using MiniEngine.Graphics.ParticipatingMedia;
using MiniEngine.Graphics.Particles;
using MiniEngine.Graphics.Physics;
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
            this.CreateModel(sponza, 0.05f);

            var entity = this.Entities.Create();

            // Add dust
            var cube = CubeGenerator.Generate(this.Device);
            this.Components.Add(ParticipatingMediaComponent.Create(entity, this.Device, cube, this.Device.Viewport.Width, this.Device.Viewport.Height, 4.0f, new Color(0.1f, 0.1f, 0.1f)));
            this.Components.Add(new TransformComponent(entity, Vector3.Zero, new Vector3(200, 150.0f, 120.0f)));

            AdditiveParticles(new Vector3(-49.0f, 3.0f, 0.0f), 1024);
        }

        private void AdditiveParticles(Vector3 position, int dim)
        {
            var particleEntity = this.Entities.Create();
            this.Components.Add(new TransformComponent(particleEntity, position, Vector3.One, Quaternion.CreateFromYawPitchRoll(0, MathHelper.PiOver2, 0)));
            var component = new ParticleEmitterComponent(particleEntity, this.Device, dim * dim);
            this.Components.Add(component);

            var mutator = new TransformMutatorComponent(particleEntity, Paths.Circle(position, 5.0f, 5.0f, 16));
            this.Components.Add(mutator);
        }

        private void CreateModel(GeometryModel model, float scale)
        {
            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, model));
            this.Components.Add(new TransformComponent(entity, Vector3.Zero, scale));
        }
    }
}
