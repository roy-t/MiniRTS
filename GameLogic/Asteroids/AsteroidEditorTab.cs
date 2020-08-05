using GameLogic.BluePrints;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Pipeline.Models.Generators;
using MiniEngine.Systems;
using MiniEngine.UI;
using MiniEngine.UI.Helpers;

namespace GameLogic.Asteroids
{
    public class AsteroidEditorTab
    {
        private readonly Editors Editors;
        private readonly SpherifiedCubeGenerator SpherifiedCubeGenerator;
        private readonly NoiseGenerator NoiseGenerator;
        private readonly EntityController EntityController;

        private readonly AsteroidBluePrint BluePrint;

        private Geometry asteroid;
        private float multiplierA = MathHelper.TwoPi;
        private float multiplierB = 0.05f;

        public AsteroidEditorTab(Editors editors, SpherifiedCubeGenerator spherifiedCubeGenerator, EntityController entityController, NoiseGenerator noiseGenerator)
        {
            this.Editors = editors;
            this.BluePrint = new AsteroidBluePrint();
            this.SpherifiedCubeGenerator = spherifiedCubeGenerator;
            this.EntityController = entityController;
            this.NoiseGenerator = noiseGenerator;
        }

        public void Edit()
        {
            ObjectEditor.Create(this.Editors, this.BluePrint);

            if (this.asteroid != null)
            {
                if (ImGui.Button("Regenerate"))
                {
                    this.EntityController.DestroyEntity(this.asteroid.Entity);
                    this.Generate();
                }

                ImGui.Separator();

                if (ImGui.SliderFloat("MultplierA", ref this.multiplierA, 0.0f, MathHelper.Pi * 10) ||
                    ImGui.SliderFloat("MultplierB", ref this.multiplierB, 0.0f, 0.2f) ||
                    ImGui.Button("Apply Noise"))
                {
                    this.NoiseGenerator.GenerateNoise(this.asteroid, new NoiseSettings() { multiplierA = multiplierA, multiplierB = multiplierB });
                }
            }
            else
            {
                if (ImGui.Button("Generate"))
                {
                    this.Generate();
                }
            }
        }

        private void Generate()
        {
            this.asteroid = this.SpherifiedCubeGenerator.Generate(this.BluePrint.Radius, this.BluePrint.Subdivisions);
        }
    }
}
