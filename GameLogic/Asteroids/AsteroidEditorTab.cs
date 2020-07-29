using GameLogic.BluePrints;
using ImGuiNET;
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

        private Entity? lastAsteroid;

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

            if (this.lastAsteroid.HasValue)
            {
                if (ImGui.Button("Regenerate"))
                {
                    this.EntityController.DestroyEntity(this.lastAsteroid.Value);
                    this.Generate();
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
            var asteroid = this.SpherifiedCubeGenerator.Generate(this.BluePrint.Radius, this.BluePrint.Subdivisions);
            this.lastAsteroid = asteroid.Entity;

            this.NoiseGenerator.GenerateNoise();
        }
    }
}
