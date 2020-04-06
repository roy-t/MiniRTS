using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Input;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
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
        private readonly List<PathFollowLogic> Followers;

        private WorldGrid worldGrid;
        private bool pause = true;

        private AModel carModel;
        private CarAnimation carAnimation;
        private DebugLine pathLine;

        public CarScene(SceneBuilder sceneBuilder, EntityController entityController, MouseInput mouseInput, KeyboardInput keyboardInput)
        {
            this.SceneBuilder = sceneBuilder;
            this.EntityController = entityController;
            this.MouseInput = mouseInput;
            this.KeyboardInput = keyboardInput;
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

            (this.carModel, this.carAnimation) = this.SceneBuilder.BuildCar(new Pose(Vector3.Zero, 0.1f));

            // TODO: fix origin in a different place!
            var carDynamics = new CarDynamics(new CarLayout(this.carModel));
            this.carModel.Origin = carDynamics.GetCarSupportedCenter();
            this.carModel.Move(this.worldGrid.ToWorldPositionCentered(new GridPosition(19, 19)));
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            if (this.KeyboardInput.Click(Microsoft.Xna.Framework.Input.Keys.P))
            {
                this.pause = !this.pause;
            }

            if (!this.pause)
            {
                for (var i = 0; i < this.Followers.Count; i++)
                {
                    this.Followers[i].Update(elapsed);
                }
            }

            if (this.MouseInput.Click(MouseButtons.Left))
            {
                this.Followers.Clear();
                if (this.pathLine != null)
                {
                    this.EntityController.DestroyEntity(this.pathLine.Entity);
                }

                var mouseWorldPosition = camera.Pick(this.MouseInput.Position, 0.0f);
                var roughPath = this.worldGrid.PlanPath(this.carModel.Position, mouseWorldPosition);

                //var waypoints = new List<Vector3>()
                //    {
                //        this.carModel.Position,
                //        camera.Pick(this.MouseInput.Position, 0.0f)
                //    };
                //var roughPath = new Path(waypoints);
                var smoothPath = PathInterpolator.Interpolate(roughPath);
                var completePath = PathStarter.CreateStart(smoothPath, this.carModel);


                this.pathLine = this.SceneBuilder.CreateDebugLine(completePath.WayPoints, Color.Purple);

                var followLogic = new PathFollowLogic(this.worldGrid, this.carModel, this.carAnimation, completePath,
                    new MetersPerSecond(0.1f));
                followLogic.Update(new Seconds(0));

                this.Followers.Add(followLogic);
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

