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
        private readonly FlightPlanFactory FlightPlanFactory;
        private WorldGrid worldGrid;

        private Pose targetPose;
        private float radius = 10.0f;
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private float linearAcceleration = 1.0f;
        private float angularAcceleration = MathHelper.PiOver4;

        private int selectedFighter;
        private List<Pose> fighters;
        private List<string> fighterNames;

        public FlightScene(
            SceneBuilder sceneBuilder,
            FlightPlanFactory flightPlanFactory)
        {
            this.SceneBuilder = sceneBuilder;
            this.FlightPlanFactory = flightPlanFactory;
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

            this.fighters = new List<Pose>();
            this.fighterNames = new List<string>();
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            var targetPosition = this.radius * Vector3.TransformNormal(Vector3.Forward, Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f));
            this.targetPose.Position = targetPosition;
        }

        public void RenderUI()
        {
            if (ImGui.Begin("Scene"))
            {
                if (ImGui.BeginTabBar("SceneTabs"))
                {
                    if (ImGui.BeginTabItem("Fighters"))
                    {
                        ImGui.ListBox("Selection", ref this.selectedFighter, this.fighterNames.ToArray(), this.fighterNames.Count);
                        ImGui.EndTabItem();

                        if (ImGui.Button("Build Fighter"))
                        {
                            var (fighterPose, _, _) = this.SceneBuilder.BuildFighter(Vector3.Zero, 1.0f);
                            this.fighters.Add(fighterPose);
                            this.fighterNames.Add($"Fighter {this.fighters.Count}");
                        }

                        if (ImGui.Button("Move"))
                        {
                            var pose = this.fighters[this.selectedFighter];
                            var maneuvers = new Queue<IManeuver>();
                            ManeuverPlanner.PlanMoveTo(maneuvers, pose, this.targetPose.Position, this.linearAcceleration, this.angularAcceleration);
                            this.FlightPlanFactory.Construct(pose.Entity, maneuvers);
                        }
                    }


                    if (ImGui.BeginTabItem("Debug"))
                    {
                        ImGui.DragFloat("Radius", ref this.radius);
                        ImGui.SliderFloat("Yaw", ref this.yaw, -MathHelper.Pi, MathHelper.Pi);
                        ImGui.SliderFloat("Pitch", ref this.pitch, -MathHelper.PiOver2 + 0.001f, MathHelper.PiOver2 - 0.001f);

                        ImGui.Spacing();

                        ImGui.SliderFloat("Linear Acceleration", ref this.linearAcceleration, 0.1f, 10.0f);
                        ImGui.SliderFloat("Angular Acceleration", ref this.angularAcceleration, 0.1f, 10.0f);

                        ImGui.Spacing();

                        if (ImGui.Button("Invert"))
                        {
                            this.yaw = MathHelper.WrapAngle(this.yaw + MathHelper.Pi);
                            this.pitch = MathHelper.WrapAngle(-this.pitch);
                        }

                        ImGui.EndTabItem();
                    }

                    ImGui.EndTabBar();
                }

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

