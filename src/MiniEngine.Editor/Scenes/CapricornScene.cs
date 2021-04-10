using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.ParticipatingMedia;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class CapricornScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly SkyboxSceneService Skybox;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly GeneratedAssets GeneratedAssets;

        public CapricornScene(GraphicsDevice device, SkyboxSceneService skybox, EntityAdministrator entities, ComponentAdministrator components, GeneratedAssets generatedAssets)
        {
            this.Device = device;
            this.Skybox = skybox;
            this.Entities = entities;
            this.Components = components;
            this.GeneratedAssets = generatedAssets;
        }

        public void RenderMainMenuItems()
            => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            var caprica = content.Load<Texture2D>("capricorn");
            var plane = CubeGenerator.Generate(this.Device);
            this.CreateSphere(plane,
                new Material
                (
                    caprica,
                    this.GeneratedAssets.NormalPixel(),
                    this.GeneratedAssets.MetalicnessPixel(1.0f),
                    this.GeneratedAssets.RoughnessPixel(0.0f),
                    this.GeneratedAssets.AmbientOcclussionPixel(1.0f)
                ), Vector3.Forward * 20, new Vector3(20, 20, 0.01f));



            // Add dust
            var entity = this.Entities.Create();
            var cube = CubeGenerator.Generate(this.Device);
            this.Components.Add(ParticipatingMediaComponent.Create(entity, this.Device, cube, this.Device.Viewport.Width, this.Device.Viewport.Height, 4.0f, new Color(0.1f, 0.1f, 0.1f)));
            this.Components.Add(new TransformComponent(entity, Vector3.Zero, new Vector3(200, 150.0f, 120.0f)));
        }

        private void CreateSphere(GeometryData geometry, Material material, Vector3 position, Vector3 scale)
        {
            var mesh = new GeometryMesh(geometry, material, Matrix.Identity);
            var model = new GeometryModel();
            model.Add(mesh);

            CreateModel(model, position, scale);
        }

        private void CreateModel(GeometryModel model, Vector3 position, Vector3 scale)
        {
            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, model));
            this.Components.Add(new TransformComponent(entity, position, scale));
        }
    }
}
