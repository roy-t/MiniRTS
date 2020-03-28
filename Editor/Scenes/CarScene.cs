using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Pipeline.Models.Components;
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

        private WorldGrid worldGrid;
        private PathFollowLogic followLogic;

        private AModel carModel;
        private CarAnimation carAnimation;

        private AModel indicator;

        private DebugLine smoothPathLine;
        private DebugLine roughPathLine;
        private System.Numerics.Vector2 endPosition;

        public CarScene(SceneBuilder sceneBuilder, EntityController controller)
        {
            this.SceneBuilder = sceneBuilder;
            this.EntityController = controller;
            this.endPosition = new System.Numerics.Vector2(10, 7);
        }

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Car";

        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.SceneBuilder.BuildTerrain(new Vector2(40, 40));
            this.SceneBuilder.BuildSponzaAmbientLight();
            this.SceneBuilder.BuildSponzeSunLight();
            this.Skybox = this.SceneBuilder.SponzaSkybox;

            this.worldGrid = new WorldGrid(40, 40, 1, 8, new Vector3(-20, 0, -20));
            this.SceneBuilder.CreateDebugLine(CreateGridLines(40, 40), Color.White);

            this.indicator = this.SceneBuilder.BuildCube(new Pose(Vector3.Zero, 0.0002f));

            (this.carModel, this.carAnimation) = this.SceneBuilder.BuildCar(new Pose(Vector3.Zero, 0.1f));

            this.CreatePath();
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

        private void CreatePath()
        {
            if (this.smoothPathLine != null)
            {
                this.EntityController.DestroyEntity(this.smoothPathLine.Entity);
                this.EntityController.DestroyEntity(this.roughPathLine.Entity);
            }

            var from = new GridPosition(0, 0);
            var to = new GridPosition((int)this.endPosition.X, (int)this.endPosition.Y);

            var roughPath = this.worldGrid.PlanPath(from, to);
            this.roughPathLine = this.SceneBuilder.CreateDebugLine(roughPath.WayPoints, Color.LightCoral);

            var smoothPath = PathInterpolator.Interpolate(roughPath);
            this.smoothPathLine = this.SceneBuilder.CreateDebugLine(smoothPath.WayPoints, Color.Red);

            this.followLogic = new PathFollowLogic(this.worldGrid, this.carModel, this.carAnimation, smoothPath, new MetersPerSecond(0.25f));
        }

        public void RenderUI()
        {
            if (ImGui.BeginMenu("Car Scene"))
            {
                ImGui.SliderFloat2("Target", ref this.endPosition, 0, 39);

                if (ImGui.MenuItem("Move"))
                {
                    this.CreatePath();

                }

                ImGui.EndMenu();
            }
        }

        public void Update(Seconds elapsed)
        {
            this.followLogic.Update(elapsed);

            var worldPosition = this.worldGrid.ToWorldPositionCentered(this.followLogic.lastReserved);
            this.indicator.Pose = new Pose(worldPosition, 0.002f);
        }
    }
}

