using System;
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
    public sealed class DustScene : IScene
    {
        private static readonly float[] DefaultCascadeDistances =
{
            0.075f,
            0.15f,
            0.3f,
            1.0f
        };

        private readonly GraphicsDevice Device;
        private readonly GeneratedAssets Assets;
        private readonly SkyboxSceneService Skybox;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public DustScene(GraphicsDevice device, GeneratedAssets assets, SkyboxSceneService skybox, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Device = device;
            this.Assets = assets;
            this.Skybox = skybox;
            this.Entities = entities;
            this.Components = components;
        }

        public void RenderMainMenuItems()
            => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            this.AddAsteroids();
            this.AddSunlight();
        }

        private void AddAsteroids()
        {
            var geometry = SphereGenerator.Generate(this.Device, 15);
            var material = new Material(this.Assets.AlbedoPixel(Color.Red), this.Assets.NormalPixel(), this.Assets.MetalicnessPixel(0.3f), this.Assets.RoughnessPixel(0.5f), this.Assets.AmbientOcclussionPixel(1.0f));
            var transform = Matrix.Identity;

            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, new GeometryModel(geometry, material)));
            this.Components.Add(new TransformComponent(entity, transform));

            var random = new Random(255);
            var transforms = new Matrix[1024];
            var i = 0;
            for (var x = -16; x < 16; x++)
            {
                for (var z = -16; z < 16; z++)
                {
                    var y = ((float)random.NextDouble() + 0.5f) * 10.0f;

                    var p = new Vector3(x * 2, y, z * 2);
                    transforms[i++] = Matrix.CreateTranslation(p);
                }
            }

            transforms[0] = Matrix.CreateScale(new Vector3(2.0f, 0.1f, 2.0f)) * Matrix.CreateTranslation(Vector3.Up * 30);

            this.Components.Add(InstancingComponent.Create(entity, transforms));
        }

        private void AddSunlight()
        {
            var entity = this.Entities.Create();
            this.Components.Add(new SunlightComponent(entity, Color.White, 3));
            this.Components.Add(CascadedShadowMapComponent.Create(entity, this.Device, 2048, DefaultCascadeDistances));

            var position = Vector3.Up;
            var lookAt = (Vector3.Left * 0.75f) + (Vector3.Backward * 0.1f);
            var forward = Vector3.Normalize(lookAt - position);

            var camera = new PerspectiveCamera(1.0f, position, forward);
            this.Components.Add(new CameraComponent(entity, camera));

            // Add dust
            var dustEntity = this.Entities.Create();
            var cube = CubeGenerator.Generate(this.Device);
            this.Components.Add(DustComponent.Create(dustEntity, this.Device, cube));
            this.Components.Add(new TransformComponent(dustEntity, Matrix.CreateScale(300.0f, 50.0f, 30.0f)));
        }
    }
}
