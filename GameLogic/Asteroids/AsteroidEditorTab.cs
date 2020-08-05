using GameLogic.BluePrints;
using ImGuiNET;
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

                if (ImGui.Button("Apply Noise"))
                {

                    this.NoiseGenerator.GenerateNoise(this.asteroid);
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
