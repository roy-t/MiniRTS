using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.GameLogic.Factories;
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
        private readonly FlightPlanFactory flightPlanFactory;
        private WorldGrid worldGrid;

        private Pose targetPose;
        private float radius = 10.0f;
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private float linearAcceleration = 1.0f;
        private float angularAcceleration = MathHelper.PiOver4;
        private bool set;
        private Pose fighterPose;

        public FlightScene(
            SceneBuilder sceneBuilder,
            FlightPlanFactory flightPlanFactory)
        {
            this.SceneBuilder = sceneBuilder;
            this.flightPlanFactory = flightPlanFactory;
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

            // TODO: for the thruster it looks best if the accelerometer is at the center of mass but the emitter should
            // be placed at the exhaust
            this.SceneBuilder.BuildThruster(fighterPose.Entity, Vector3.Backward * 0, MathHelper.Pi, 0, 0);

            this.fighterPose = fighterPose;
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            var targetPosition = (this.radius * Vector3.TransformNormal(Vector3.Forward, Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f)));
            this.targetPose.Position = targetPosition;

            if (this.set)
            {
                var maneuvers = new Queue<IManeuver>();
                ManeuverPlanner.PlanMoveTo(maneuvers, this.fighterPose, targetPosition, this.linearAcceleration, this.angularAcceleration);

                this.flightPlanFactory.Construct(this.fighterPose.Entity, maneuvers);
                this.set = false;
            }
        }

        public void RenderUI()
        {
            if (ImGui.Begin("Scene"))
            {
                ImGui.DragFloat("Radius", ref this.radius);
                ImGui.SliderFloat("Yaw", ref this.yaw, -MathHelper.Pi, MathHelper.Pi);
                ImGui.SliderFloat("Pitch", ref this.pitch, -MathHelper.PiOver2 + 0.001f, MathHelper.PiOver2 - 0.001f);

                ImGui.Spacing();

                ImGui.SliderFloat("Linear Acceleration", ref this.linearAcceleration, 0.1f, 10.0f);
                ImGui.SliderFloat("Angular Acceleration", ref this.angularAcceleration, 0.1f, 10.0f);

                ImGui.Spacing();

                this.set = ImGui.Button("Go!");

                if (ImGui.Button("Invert"))
                {
                    this.yaw = MathHelper.WrapAngle(this.yaw + MathHelper.Pi);
                    this.pitch = MathHelper.WrapAngle(-this.pitch);
                }

                ImGui.Spacing();

                ImGui.Text($"Distance to target: {Vector3.Distance(this.fighterPose.Position, this.targetPose.Position):F2}");

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

