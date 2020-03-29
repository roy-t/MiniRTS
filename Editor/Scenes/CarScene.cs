using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Units;
using Roy_T.AStar.Primitives;

namespace MiniEngine.Scenes
{
    public sealed class CarScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;
        private readonly EntityController EntityController;
        private readonly List<PathFollowLogic> Followers;

        private WorldGrid worldGrid;
        private bool pause = false;

        public CarScene(SceneBuilder sceneBuilder, EntityController controller)
        {
            this.SceneBuilder = sceneBuilder;
            this.EntityController = controller;
            this.Followers = new List<PathFollowLogic>();
        }

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Car";

        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.Followers.Clear();

            this.SceneBuilder.BuildTerrain(new Vector2(40, 40));
            this.SceneBuilder.BuildSponzaAmbientLight();
            this.SceneBuilder.BuildSponzeSunLight();
            this.Skybox = this.SceneBuilder.SponzaSkybox;

            this.worldGrid = new WorldGrid(40, 40, 1, 8, new Vector3(-20, 0, -20));
            this.SceneBuilder.CreateDebugLine(CreateGridLines(40, 40), Color.White);

            const int n = 40;
            for (var x = 0; x < n; x++)
            {
                (var carModel, var carAnimation) = this.SceneBuilder.BuildCar(new Pose(Vector3.Zero, 0.1f));

                var from = new GridPosition(x, 0);
                var to = new GridPosition(x, 39);
                var roughPath = this.worldGrid.PlanPath(from, to);
                var smoothPath = PathInterpolator.Interpolate(roughPath);

                var followLogic = new PathFollowLogic(this.worldGrid, carModel, carAnimation, smoothPath, new MetersPerSecond(0.25f));
                this.Followers.Add(followLogic);
            }

            for (var y = 0; y < n; y++)
            {
                (var carModel, var carAnimation) = this.SceneBuilder.BuildCar(new Pose(Vector3.Zero, 0.1f));

                var from = new GridPosition(0, y);
                var to = new GridPosition(39, y);
                var roughPath = this.worldGrid.PlanPath(from, to);
                var smoothPath = PathInterpolator.Interpolate(roughPath);

                var followLogic = new PathFollowLogic(this.worldGrid, carModel, carAnimation, smoothPath, new MetersPerSecond(0.25f));
                this.Followers.Add(followLogic);
            }
        }

        public void Update(Seconds elapsed)
        {
            if (!this.pause)
            {
                for (var i = 0; i < this.Followers.Count; i++)
                {
                    this.Followers[i].Update(elapsed);
                }
            }
        }

        public void RenderUI()
        {
            if (ImGui.BeginMenu("Car Scene"))
            {
                if (ImGui.MenuItem("Pause"))
                {
                    this.pause = !this.pause;
                }

                //ImGui.SliderFloat2("Target", ref this.endPosition, 0, 39);

                //if (ImGui.MenuItem("Move"))
                //{
                //    this.CreatePath();

                //}

                ImGui.EndMenu();
            }
        }

        private IReadOnlyList<Vector3> CreateGridLines(int columns, int rows)
        {
            var vertices = new List<Vector3>();

            var turn = 0;
            for (var x = 0; x <= columns; turn++)
            {
                Vector3 worldPosition;
                switch (turn % 4)
                {
                    case 0:
                        worldPosition = this.worldGrid.ToWorldPosition(new GridPosition(x, 0));
                        break;
                    case 1:
                        worldPosition = this.worldGrid.ToWorldPosition(new GridPosition(x, rows));
                        x++;
                        break;
                    case 2:
                        worldPosition = this.worldGrid.ToWorldPosition(new GridPosition(x, rows));
                        break;
                    case 3:
                    default:
                        worldPosition = this.worldGrid.ToWorldPosition(new GridPosition(x, 0));
                        x++;
                        break;
                }
                vertices.Add(worldPosition);
            }

            turn = 0;
            for (var y = 0; y <= rows; turn++)
            {
                Vector3 worldPosition;
                switch (turn % 4)
                {
                    case 0:
                        worldPosition = this.worldGrid.ToWorldPosition(new GridPosition(0, y));
                        break;
                    case 1:
                        worldPosition = this.worldGrid.ToWorldPosition(new GridPosition(columns, y));
                        y++;
                        break;
                    case 2:
                        worldPosition = this.worldGrid.ToWorldPosition(new GridPosition(columns, y));
                        break;
                    case 3:
                    default:
                        worldPosition = this.worldGrid.ToWorldPosition(new GridPosition(0, y));
                        y++;
                        break;
                }
                vertices.Add(worldPosition);
            }

            return vertices;
        }
    }
}

