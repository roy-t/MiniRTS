using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;
using Roy_T.AStar.Primitives;

namespace MiniEngine.Scenes
{
    public sealed class FlightScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;
        private WorldGrid worldGrid;

        private AttitudeController attitudeController;
        private Pose targetPose;
        float radius = 10.0f;
        float yaw = 0.0f;
        float pitch = 0.0f;
        float x, y, z = 0.0f;

        public FlightScene(
            SceneBuilder sceneBuilder)
        {
            this.SceneBuilder = sceneBuilder;
        }

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Flight Scene";

        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.SceneBuilder.BuildSponzaAmbientLight();
            this.SceneBuilder.BuildSponzeSunLight();
            this.Skybox = this.SceneBuilder.SponzaSkybox;

            this.worldGrid = new WorldGrid(40, 40, 1, 8, new Vector3(-20, 0, -20));

            this.SceneBuilder.CreateDebugLine(this.CreateGridLines(40, 40), Color.White);

            var (cubePose, _, _) = this.SceneBuilder.BuildCube(Vector3.Zero, 0.005f);
            this.targetPose = cubePose;

            var (fighterPose, _, _) = this.SceneBuilder.BuildFighter(Vector3.Zero, 1.0f);
            this.SceneBuilder.BuildSmallReactionControlSystem(fighterPose.Entity, Vector3.Forward * 4, 0, 0, 0);
            this.SceneBuilder.BuildSmallReactionControlSystem(fighterPose.Entity, Vector3.Backward * 4, 0, 0, 0);
            this.SceneBuilder.BuildThruster(fighterPose.Entity, Vector3.Backward * 4, MathHelper.Pi, 0, 0);

            this.attitudeController = new AttitudeController(fighterPose);
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            var targetPosition = new Vector3(this.x, this.y, this.z);
            this.targetPose.Position = targetPosition + (this.radius * Vector3.TransformNormal(Vector3.Forward, Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f)));

            this.attitudeController.PointAt = this.targetPose.Position;
            this.attitudeController.MoveTo = targetPosition;
            this.attitudeController.Update(elapsed);
        }

        public void RenderUI()
        {
            if (ImGui.Begin("Scene"))
            {
                ImGui.DragFloat("X", ref this.x);
                ImGui.DragFloat("Y", ref this.y);
                ImGui.DragFloat("Z", ref this.z);

                ImGui.Spacing();

                ImGui.SliderFloat("Radius", ref this.radius, 0.0f, 10.0f);
                ImGui.SliderFloat("Yaw", ref this.yaw, -MathHelper.Pi, MathHelper.Pi);
                ImGui.SliderFloat("Pitch", ref this.pitch, -MathHelper.PiOver2 + 0.001f, MathHelper.PiOver2 - 0.001f);

                ImGui.End();
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

