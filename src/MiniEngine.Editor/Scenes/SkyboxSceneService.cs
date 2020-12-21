using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Skybox;
using MiniEngine.Graphics.Utilities;
using MiniEngine.SceneManagement;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SkyboxSceneService
    {
        private sealed record SkyboxTextures(string Name, TextureCube Albedo, TextureCube Irradiance, TextureCube Environment);

        private readonly FrameService FrameService;

        private readonly List<SkyboxTextures> Textures;
        private int selectedSkybox;

        public SkyboxSceneService(GraphicsDevice device, ContentStack content, CubeMapGenerator cubeMapGenerator, IrradianceMapGenerator irradianceMapGenerator, EnvironmentMapGenerator environmentMapGenerator, FrameService frameService)
        {
            this.Textures = new List<SkyboxTextures>();
            this.FrameService = frameService;

            content.Push("skybox");
            this.CreateSkyboxes(device, content, frameService, cubeMapGenerator, irradianceMapGenerator, environmentMapGenerator);
        }

        public void SelectSkybox()
        {
            if (ImGui.BeginMenu("Environment"))
            {
                var names = this.Textures.Select(t => t.Name).ToArray();
                if (ImGui.ListBox("Skybox", ref this.selectedSkybox, names, names.Length))
                {
                    this.SetSkyboxTexture(this.Textures[this.selectedSkybox]);
                }

                var value = this.FrameService.Skybox.AmbientLightFactor;
                if (ImGui.SliderFloat("Ambient Light", ref value, 0.0f, 2.0f))
                {
                    this.FrameService.Skybox.AmbientLightFactor = value;
                }

                ImGui.EndMenu();
            }
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
