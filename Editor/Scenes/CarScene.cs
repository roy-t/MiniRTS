using System;
using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.GameLogic.Vehicles;
using MiniEngine.Input;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;
using Roy_T.AStar.Primitives;

namespace MiniEngine.Scenes
{
    public sealed class CarScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;
        private readonly EntityController EntityController;
        private readonly MouseInput MouseInput;
        private readonly KeyboardInput KeyboardInput;

        private WorldGrid worldGrid;
        private bool pause = true;

        private DebugLine pathLine;
        private UVAnimation tankTrackAnimation;
        private Tank tank;
        private TankPathFollowLogic tankPathFollowLogic;


        public CarScene(
            SceneBuilder sceneBuilder,
            EntityController entityController,
            MouseInput mouseInput,
            KeyboardInput keyboardInput)
        {
            this.SceneBuilder = sceneBuilder;
            this.EntityController = entityController;
            this.MouseInput = mouseInput;
            this.KeyboardInput = keyboardInput;
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

            var (pose, model, bounds, animation) = this.SceneBuilder.BuildTank(Vector3.Zero, 0.2f);


            var (fighter_pose, fighter_model, fighter_bounds) = this.SceneBuilder.BuildFighter(Vector3.Zero, 1.0f);


            this.tank = new Tank(model, bounds, pose, animation);
            this.tankTrackAnimation = animation;
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            return;

            if (this.KeyboardInput.Click(Microsoft.Xna.Framework.Input.Keys.P))
            {
                this.pause = !this.pause;
            }

            if (!this.pause)
            {
                if (this.tankPathFollowLogic != null)
                {
                    this.tankPathFollowLogic.Update(elapsed);
                }
            }

            if (this.MouseInput.Click(MouseButtons.Left))
            {
                if (this.pathLine != null)
                {
                    this.EntityController.DestroyEntity(this.pathLine.Entity);
                }

                var mouseWorldPosition = camera.Pick(this.MouseInput.Position, 0.0f);
                var roughPath = this.worldGrid.PlanPath(this.tank.Pose.Position, mouseWorldPosition);
                var smoothPath = PathInterpolator.Interpolate(roughPath);

                //var smoothPath = CreateCirclePath();
                //var completePath = PathStarter.CreateStart(smoothPath, this.car);

                this.pathLine = this.SceneBuilder.CreateDebugLine(smoothPath.WayPoints, Color.Purple);

                this.tankPathFollowLogic = new TankPathFollowLogic(worldGrid, tank, smoothPath, new MetersPerSecond(0.08f));

                //var followLogic = new PathFollowLogic(this.worldGrid, this.car, smoothPath,
                //    new MetersPerSecond(0.1f));
                //followLogic.Update(new Seconds(0));

                //this.Followers.Add(followLogic);
            }

        }

        private static Path CreateCirclePath()
        {
            var waypoints = new List<Vector3>();

            for (var i = 0; i < 360; i++)
            {
                var x = (float)Math.Cos(MathHelper.ToRadians(i));
                var y = (float)Math.Sin(MathHelper.ToRadians(i));
                waypoints.Add(new Vector3(x, 0, y) * 3);
            }
            var smoothPath = new Path(waypoints);
            return smoothPath;
        }

        public void RenderUI()
        {
            if (ImGui.BeginMenu("Car Scene"))
            {
                if (ImGui.MenuItem("Pause"))
                {
                    this.pause = !this.pause;
                }

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

