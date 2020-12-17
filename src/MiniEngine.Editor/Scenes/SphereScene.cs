using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Graphics.Skybox;
using MiniEngine.Graphics.Utilities;
using MiniEngine.SceneManagement;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SphereScene : IScene
    {
        private sealed record SkyboxTextures(string Name, TextureCube Albedo, TextureCube Irradiance, TextureCube Environment);

        private readonly GraphicsDevice Device;
        private readonly ContentStack Content;
        private readonly FrameService FrameService;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        private readonly List<SkyboxTextures> Textures;
        private int selectedSkybox;

        public SphereScene(GraphicsDevice device, ContentStack content, FrameService frameService, CubeMapGenerator cubeMapGenerator, IrradianceMapGenerator irradianceMapGenerator, EnvironmentMapGenerator environmentMapGenerator, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Device = device;
            this.Content = content;
            this.FrameService = frameService;
            this.Entities = entities;
            this.Components = components;

            this.Textures = new List<SkyboxTextures>();

            content.Push("sphere-scene");

            var model = content.Load<Pose>("sponza/sponza");

            this.CreateSkyboxes(device, content, frameService, cubeMapGenerator, irradianceMapGenerator, environmentMapGenerator);

            var red = new Texture2D(device, 1, 1);
            red.SetData(new Color[] { Color.White });
            content.Link(red);

            var normal = new Texture2D(device, 1, 1);
            normal.SetData(new Color[] { new Color(0.5f, 0.5f, 1.0f) });
            content.Link(normal);

            var blue = content.Load<Texture2D>("Textures/Blue");
            var bumps = content.Load<Texture2D>("Textures/Bricks_Normal");

            var rows = 7;
            var columns = 7;
            var spacing = 2.5f;
            var geometry = SphereGenerator.Generate(device, 15);
            for (var row = 0; row < rows; row++)
            {
                var metalicness = row / (float)rows;
                for (var col = 0; col < columns; col++)
                {
                    var roughness = Math.Clamp(col / (float)columns, 0.05f, 1.0f);
                    var material = new Material(red, normal, metalicness, roughness);

                    var position = new Vector3((col - (columns / 2.0f)) * spacing, (row - (rows / 2.0f)) * spacing, 0.0f);
                    var transform = Matrix.CreateTranslation(position);
                    this.CreateSphere(geometry, material, transform);
                }
            }

            var backgroundGeometry = CubeGenerator.Generate(device);
            this.CreateSphere(backgroundGeometry, new Material(blue, bumps, 0.0f, 1.0f), Matrix.CreateScale(200, 200, 1) * Matrix.CreateTranslation(Vector3.Forward * 20));

            this.CreateLight(new Vector3(-10, 10, 10), Color.Red, 30.0f);
            this.CreateLight(new Vector3(10, 10, 10), Color.Blue, 30.0f);
            this.CreateLight(new Vector3(-10, -10, 10), Color.Green, 30.0f);
            this.CreateLight(new Vector3(10, -10, 10), Color.White, 30.0f);

            this.CreateSpotLight(new Vector3(0, 0, 10), Vector3.Forward, 1500.0f);
        }

        public void RenderMainMenuItems()
        {
            if (ImGui.BeginMenu("Environment"))
            {
                var names = this.Textures.Select(t => t.Name).ToArray();
                if (ImGui.ListBox("Skybox", ref this.selectedSkybox, names, names.Length))
                {
                    this.SetSkyboxTexture(this.Textures[this.selectedSkybox]);
                }
                ImGui.EndMenu();
            }
        }

        private void CreateSphere(GeometryData geometry, Material material, Matrix transform)
        {
            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, geometry, material));
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
            this.Components.Add(ShadowMapComponent.Create(this.Device, entity, 1024));
            this.Components.Add(new CameraComponent(entity, new PerspectiveCamera(this.Device.Viewport.AspectRatio, position, forward)));
            this.Components.Add(new SpotLightComponent(entity, Color.Yellow, strength));
        }

        private void SetSkyboxTexture(SkyboxTextures texture)
        {
            this.FrameService.Skybox.Texture = texture.Albedo;
            this.FrameService.Skybox.Irradiance = texture.Irradiance;
            this.FrameService.Skybox.Environment = texture.Environment;
        }

        private void CreateSkyboxes(GraphicsDevice device, ContentStack content, FrameService frameService, CubeMapGenerator cubeMapGenerator, IrradianceMapGenerator irradianceMapGenerator, EnvironmentMapGenerator environmentMapGenerator)
        {
            var skyboxNames = new string[]
                        {
                "Skyboxes/Circus/Circus_Backstage_3k",
                "Skyboxes/Industrial/fin4_Bg",
                "Skyboxes/Milkyway/Milkyway_small",
                "Skyboxes/Grid/testgrid",
                "Skyboxes/Loft/Newport_Loft_Ref"
                        };

            foreach (var name in skyboxNames)
            {
                content.Push("generator");
                var equiRect = content.Load<Texture2D>(name);
                var albedo = cubeMapGenerator.Generate(equiRect);
                var irradiance = irradianceMapGenerator.Generate(equiRect);
                var environment = environmentMapGenerator.Generate(equiRect);

                content.Pop();
                content.Link(albedo);
                content.Link(irradiance);
                content.Link(environment);

                this.Textures.Add(new SkyboxTextures(name, albedo, irradiance, environment));
            }

            frameService.Skybox = SkyboxGenerator.Generate(device, this.Textures[0].Albedo,
                this.Textures[0].Irradiance,
                this.Textures[0].Environment);
        }
    }
}
