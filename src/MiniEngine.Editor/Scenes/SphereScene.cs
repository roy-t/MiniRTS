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
using MiniEngine.Graphics.Particles;
using MiniEngine.Graphics.Particles.Functions;
using MiniEngine.Graphics.Shadows;
using MiniEngine.SceneManagement;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SphereScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly SkyboxSceneService Skybox;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public SphereScene(GraphicsDevice device, SkyboxSceneService skybox, EntityAdministrator entities, ComponentAdministrator components)
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
            var red = new Texture2D(this.Device, 1, 1);
            red.SetData(new Color[] { Color.White });
            content.Link(red);

            var white = new Texture2D(this.Device, 1, 1);
            white.SetData(new Color[] { Color.White });
            content.Link(white);

            var black = new Texture2D(this.Device, 1, 1);
            black.SetData(new Color[] { Color.Black });
            content.Link(black);

            var normal = new Texture2D(this.Device, 1, 1);
            normal.SetData(new Color[] { new Color(0.5f, 0.5f, 1.0f) });
            content.Link(normal);

            var blue = content.Load<Texture2D>("Textures/Blue");
            var bumps = content.Load<Texture2D>("Textures/Bricks_Normal");

            var rows = 7;
            var columns = 7;
            var spacing = 2.5f;
            var geometry = SphereGenerator.Generate(this.Device, 15);
            for (var row = 0; row < rows; row++)
            {
                var metalicness = row / (float)rows;

                var metalicnessTexture = new Texture2D(this.Device, 1, 1);
                metalicnessTexture.SetData(new Color[] { new Color(Vector3.One * metalicness) });
                content.Link(metalicnessTexture);

                for (var col = 0; col < columns; col++)
                {
                    var roughness = Math.Clamp(col / (float)columns, 0.05f, 1.0f);
                    var roughnessTexture = new Texture2D(this.Device, 1, 1);
                    roughnessTexture.SetData(new Color[] { new Color(Vector3.One * roughness) });
                    content.Link(roughnessTexture);

                    var material = new Material(red, normal, metalicnessTexture, roughnessTexture, white);

                    var position = new Vector3((col - (columns / 2.0f)) * spacing, (row - (rows / 2.0f)) * spacing, 0.0f);
                    var transform = Matrix.CreateTranslation(position);
                    this.CreateSphere(geometry, material, transform);
                }
            }

            var backgroundGeometry = CubeGenerator.Generate(this.Device);
            this.CreateSphere(backgroundGeometry, new Material(blue, bumps, black, white, white), Matrix.CreateScale(200, 200, 1) * Matrix.CreateTranslation(Vector3.Forward * 20));

            this.CreateLight(new Vector3(-10, 10, 10), Color.Red, 30.0f);
            this.CreateLight(new Vector3(10, 10, 10), Color.Blue, 30.0f);
            this.CreateLight(new Vector3(-10, -10, 10), Color.Green, 30.0f);
            this.CreateLight(new Vector3(10, -10, 10), Color.White, 30.0f);

            this.CreateSpotLight(new Vector3(0, 0, 10), Vector3.Forward, 1500.0f);

            this.AdditiveParticles(content);
        }

        private void AdditiveParticles(ContentStack content)
        {
            var particle = content.Load<Texture2D>("Textures/AdditiveParticle");
            var particleEntity = this.Entities.Create();
            this.Components.Add(new TransformComponent(particleEntity, Matrix.CreateTranslation(Vector3.Left * 5) * Matrix.CreateRotationX(MathHelper.PiOver2)));
            var spawn = new IntervalSpawnFunction();
            //var spawn = new InstantSpawnFunction();
            var update = new LinearUpdateFunction();

            var component = new ParticleFountainComponent(particleEntity, particle.GraphicsDevice);
            component.AddEmitter(particle, spawn, update);

            this.Components.Add(component);
        }

        private void CreateSphere(GeometryData geometry, Material material, Matrix transform)
        {
            var mesh = new GeometryMesh(geometry, material, Matrix.Identity);
            var model = new GeometryModel();
            model.Add(mesh);

            CreateModel(model, transform);
        }

        private void CreateModel(GeometryModel model, Matrix transform)
        {
            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, model));
            this.Components.Add(new TransformComponent(entity, transform));
        }

        private void CreateLight(Vector3 position, Color color, float strength)
        {
            var entity = this.Entities.Create();
            this.Components.Add(new PointLightComponent(entity, color, strength));
            this.Components.Add(new TransformComponent(entity, Matrix.CreateTranslation(position)));
        }

        private void CreateSpotLight(Vector3 position, Vector3 forward, float strength)
        {
            var entity = this.Entities.Create();
            this.Components.Add(ShadowMapComponent.Create(entity, this.Device, 1024));
            this.Components.Add(new CameraComponent(entity, new PerspectiveCamera(this.Device.Viewport.AspectRatio, position, forward)));
            this.Components.Add(new SpotLightComponent(entity, Color.Yellow, strength));
        }
    }
}
