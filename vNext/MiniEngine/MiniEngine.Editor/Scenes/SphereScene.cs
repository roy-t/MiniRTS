using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Lighting;
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

        private readonly List<SkyboxTextures> Textures;
        private readonly FrameService FrameService;
        private int selectedSkybox;

        public SphereScene(GraphicsDevice device, ContentStack content, FrameService frameService, CubeMapGenerator cubeMapGenerator, IrradianceMapGenerator irradianceMapGenerator, EnvironmentMapGenerator environmentMapGenerator, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Textures = new List<SkyboxTextures>();

            content.Push("sphere-scene");

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
                    CreateSphere(entities, components, geometry, material, transform);
                }
            }

            var backgroundGeometry = CubeGenerator.Generate(device);
            CreateSphere(entities, components, backgroundGeometry, new Material(blue, bumps, 1.0f, 0.1f), Matrix.CreateScale(20, 20, 1) * Matrix.CreateTranslation(Vector3.Forward * 20));

            var pointLightComponent = new PointLightComponent(entities.Create(), new Vector3(-10, 10, 10), Color.Red, 300.0f);
            components.Add(pointLightComponent);

            var pointLightComponent2 = new PointLightComponent(entities.Create(), new Vector3(10, 10, 10), Color.Blue, 300.0f);
            components.Add(pointLightComponent2);

            var pointLightComponent3 = new PointLightComponent(entities.Create(), new Vector3(-10, -10, 10), Color.Green, 300.0f);
            components.Add(pointLightComponent3);

            var pointLightComponent4 = new PointLightComponent(entities.Create(), new Vector3(10, -10, 10), Color.White, 300.0f);
            components.Add(pointLightComponent4);
            this.FrameService = frameService;
        }

        private void SetSkyboxTexture(SkyboxTextures texture)
        {
            this.FrameService.Skybox.Texture = texture.Albedo;
            this.FrameService.Skybox.Irradiance = texture.Irradiance;
            this.FrameService.Skybox.Environment = texture.Environment;
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

        private static void CreateSphere(EntityAdministrator entities, ComponentAdministrator components, Geometry geometry, Material material, Matrix transform)
        {
            var entity = entities.Create();
            var component = new GeometryComponent(entity, geometry, material);
            components.Add(component);

            var body = new TransformComponent(entity, transform);
            components.Add(body);
        }
    }
}
