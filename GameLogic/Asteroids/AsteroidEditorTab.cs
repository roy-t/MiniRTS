using System.Collections.Generic;
using System.Linq;
using GameLogic.BluePrints;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
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

        private readonly AsteroidBluePrint AsteroidBluePrint;
        private readonly List<CraterBluePrint> CraterBluePrints;
        private readonly Texture2D Texture;

        private Geometry asteroid;
        private float rimWidth = 2.0f;
        private float rimSteepness = 0.1f;

        public AsteroidEditorTab(Content content, Editors editors, SpherifiedCubeGenerator spherifiedCubeGenerator, EntityController entityController, NoiseGenerator noiseGenerator)
        {
            this.Texture = content.DebugTexture;
            this.Editors = editors;
            this.AsteroidBluePrint = new AsteroidBluePrint();
            this.CraterBluePrints = new List<CraterBluePrint>();
            this.SpherifiedCubeGenerator = spherifiedCubeGenerator;
            this.EntityController = entityController;
            this.NoiseGenerator = noiseGenerator;

            this.CraterBluePrints.Add(new CraterBluePrint());
        }

        public void Edit()
        {


            if (this.asteroid != null)
            {
                if (ImGui.Button("Regenerate"))
                {
                    this.EntityController.DestroyEntity(this.asteroid.Entity);
                    this.Generate();
                }

                ImGui.Separator();

                if (ImGui.SliderFloat("Rim Width", ref this.rimWidth, 0.0f, 10.0f) ||
                    ImGui.SliderFloat("Rim Steepness", ref this.rimSteepness, 0.01f, 1.0f) ||
                    ImGui.Button("Apply Noise"))
                {
                    var craters = this.GenerateCraters();
                    this.NoiseGenerator.GenerateNoise(this.asteroid,
                        new NoiseSettings()
                        {
                            rimWidth = rimWidth,
                            rimSteepness = rimSteepness,
                            craterCount = craters.Length
                        },
                        craters
                    );
                }
            }
            else
            {
                if (ImGui.Button("Generate"))
                {
                    this.Generate();
                }
            }

            if (ImGui.TreeNode("Asteroid"))
            {
                ObjectEditor.Create(this.Editors, this.AsteroidBluePrint);
                ImGui.TreePop();
            }
            for (var i = 0; i < this.CraterBluePrints.Count; i++)
            {
                if (ImGui.TreeNode($"Crater {i}"))
                {
                    ObjectEditor.Create(this.Editors, this.CraterBluePrints[i]);
                    ImGui.TreePop();
                }
            }
        }

        private void Generate()
        {
            this.asteroid = this.SpherifiedCubeGenerator.Generate(this.AsteroidBluePrint.Radius, this.AsteroidBluePrint.Subdivisions, this.Texture);
        }

        private Crater[] GenerateCraters()
        {
            return this.CraterBluePrints.Select(c => c.ToCrater()).ToArray();
        }
    }
}
