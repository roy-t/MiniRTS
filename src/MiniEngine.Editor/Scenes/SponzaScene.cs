using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.ParticipatingMedia;
using MiniEngine.Graphics.Shadows;
using MiniEngine.SceneManagement;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SponzaScene : IScene
    {
        private static readonly float[] DefaultCascadeDistances =
        {
            0.075f,
            0.15f,
            0.3f,
            1.0f
        };
        private readonly GraphicsDevice Device;
        private readonly SkyboxSceneService Skybox;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public SponzaScene(GraphicsDevice device, SkyboxSceneService skybox, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Device = device;
            this.Skybox = skybox;
            this.Entities = entities;
            this.Components = components;
        }

        public void RenderMainMenuItems()
           => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            var sponza = content.Load<GeometryModel>("sponza/sponza");
            this.CreateModel(sponza, Matrix.CreateScale(0.05f));

            //var entity = this.EntityController.CreateEntity();
            //return this.GetFactory<SunlightFactory>().Construct(entity, Color.White, 
            // Vector3.Up, (Vector3.Left * 0.75f) + (Vector3.Backward * 0.1f));

            var entity = this.Entities.Create();
            this.Components.Add(new SunlightComponent(entity, Color.White, 3));
            this.Components.Add(CascadedShadowMapComponent.Create(entity, this.Device, 2048, DefaultCascadeDistances));

            var position = Vector3.Up;
            var lookAt = (Vector3.Left * 0.75f) + (Vector3.Backward * 0.1f);
            var forward = Vector3.Normalize(lookAt - position);

            var camera = new PerspectiveCamera(1.0f, position, forward);
            this.Components.Add(new CameraComponent(entity, camera));

            // Add dust
            var cube = CubeGenerator.Generate(this.Device);
            this.Components.Add(ParticipatingMediaComponent.Create(entity, this.Device, cube, 4.0f, new Color(0.1f, 0.1f, 0.1f)));
            this.Components.Add(new TransformComponent(entity, Matrix.CreateScale(200, 150.0f, 120.0f)));
        }

        private void CreateModel(GeometryModel model, Matrix transform)
        {
            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, model));
            this.Components.Add(new TransformComponent(entity, transform));
        }
    }
}
