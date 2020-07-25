using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.GameLogic.Commands;
using MiniEngine.GameLogic.Components;
using MiniEngine.GameLogic.Factories;
using MiniEngine.GameLogic.Systems;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Basics.Systems;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives.VertexTypes;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;
using MiniEngine.UI.Input;
using MiniEngine.Units;
using Roy_T.AStar.Primitives;

namespace MiniEngine.Scenes
{
    public sealed class FlightScene : IScene
    {
        private readonly Content Content;
        private readonly EntityController EntityController;
        private readonly Resolver<IComponentFactory> Factories;
        private readonly Resolver<IComponentContainer> Containers;
        private readonly SceneBuilder SceneBuilder;

        private readonly FlightPlanSystem FlightPlanSystem;
        private readonly IComponentContainer<FlightPlan> FlightPlans;
        private readonly ReactionControlSystem ReactionControlSystem;
        private readonly SelectionSystem SelectionSystem;
        private WorldGrid worldGrid;

        private Pose targetPose;
        private float radius = 10.0f;
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private float linearAcceleration = 1.0f;
        private float angularAcceleration = MathHelper.Pi * 0.04f;

        private int selectedFighter;
        private List<Entity> fighters;
        private List<string> fighterNames;

        public FlightScene(
            Content content,
            EntityController entityController,
            Resolver<IComponentFactory> factories,
            Resolver<IComponentContainer> containers,
            SceneBuilder sceneBuilder,
            FlightPlanSystem flightPlanSystem,
            IComponentContainer<FlightPlan> flightPlans,
            ReactionControlSystem reactionControlSystem,
            SelectionSystem selectionSystem)
        {
            this.Content = content;
            this.EntityController = entityController;
            this.Factories = factories;
            this.Containers = containers;
            this.SceneBuilder = sceneBuilder;
            this.FlightPlanSystem = flightPlanSystem;
            this.FlightPlans = flightPlans;
            this.ReactionControlSystem = reactionControlSystem;
            this.SelectionSystem = selectionSystem;
        }

        public void LoadContent(Content content)
            => this.Skybox = content.SponzaSkybox;

        public string Name => "Flight Scene";

        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.SceneBuilder.BuildSponzaAmbientLight();
            this.SceneBuilder.BuildSponzaSunLight();

            this.worldGrid = new WorldGrid(40, 40, 1, 8, new Vector3(-20, 0, -20));

            var lines = this.SceneBuilder.CreateDebugLine(this.CreateGridLines(40, 40), Color.White);
            lines.ClippedTint = Color.Transparent;

            var (cubePose, _, _) = this.SceneBuilder.BuildCube(Vector3.Zero, 0.005f);
            this.targetPose = cubePose;

            this.fighters = new List<Entity>();
            this.fighterNames = new List<string>();

            this.BuildSpaceShip();
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            this.FlightPlanSystem.Update(camera, elapsed);
            this.ReactionControlSystem.Update(camera, elapsed);

            var targetPosition = this.radius * Vector3.TransformNormal(Vector3.Forward, Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f));
            this.targetPose.Position = targetPosition;
        }


        bool selecting;

        public void HandleInput(PerspectiveCamera camera, KeyboardInput keyboard, MouseInput mouse)
        {
            if (mouse.JustPressed(MouseButtons.Left))
            {
                this.selecting = true;
                this.SelectionSystem.StartSelection(camera, mouse.Position);

                Console.WriteLine("Selection started");
            }
            else if (this.selecting && !mouse.Hold(MouseButtons.Left))
            {
                this.selecting = false;
                var selected = new List<Entity>();
                this.SelectionSystem.EndSelection(camera, mouse.Position, selected);

                foreach (var entity in selected)
                {
                    Console.WriteLine($"Selected {entity}");
                    this.selectedFighter = this.fighters.IndexOf(entity);
                }

                Console.WriteLine("Selection finished");
            }


            if (mouse.Click(MouseButtons.Right))
            {
                var mouseWorldPosition = camera.Pick(mouse.Position, 0.0f);
                if (mouseWorldPosition.HasValue)
                {
                    var entity = this.fighters[this.selectedFighter];

                    if (this.FlightPlans.TryGet(entity, out var flightPlan))
                    {
                        var finalManeuver = (LerpManeuver)flightPlan.Maneuvers.Last();
                        ManeuverPlanner.PlanMoveTo(flightPlan.Maneuvers, finalManeuver.TargetPosition, finalManeuver.TargetYaw, finalManeuver.TargetPitch, mouseWorldPosition.Value, this.linearAcceleration, this.angularAcceleration);
                    }
                    else
                    {
                        var pose = this.Containers.Get<ComponentContainer<Pose>>().Get(entity);
                        var maneuvers = new Queue<IManeuver>();
                        ManeuverPlanner.PlanMoveTo(maneuvers, pose.Position, pose.Yaw, pose.Pitch, mouseWorldPosition.Value, this.linearAcceleration, this.angularAcceleration);

                        this.Factories.Get<FlightPlanFactory>().Construct(entity, maneuvers);
                    }
                }
            }
        }

        public void RenderUI()
        {
            if (ImGui.Begin("Scene"))
            {
                if (ImGui.BeginTabBar("SceneTabs"))
                {
                    if (ImGui.BeginTabItem("Geometry"))
                    {
                        if (ImGui.Button("Generate"))
                        {
                            var entity = this.EntityController.CreateEntity();
                            this.Factories.Get<PoseFactory>().Construct(entity, Vector3.Zero);
                            var vertices = new GBufferVertex[]
                            {
                                new GBufferVertex(Vector3.Left),
                                new GBufferVertex(Vector3.Up),
                                new GBufferVertex(Vector3.Right),
                            };

                            var indices = new short[] { 0, 1, 2 };
                            this.Factories.Get<GeometryFactory>().Construct(entity, vertices, indices);
                        }
                        ImGui.EndTabItem();
                    }

                    if (ImGui.BeginTabItem("Fighters"))
                    {
                        ImGui.ListBox("Selection", ref this.selectedFighter, this.fighterNames.ToArray(), this.fighterNames.Count);
                        ImGui.EndTabItem();

                        if (ImGui.Button("Build Fighter"))
                        {
                            var command = new BuildFighterCommand
                            {
                                Position = Vector3.Zero,
                                Scale = Vector3.One
                            };

                            var entity = command.Execute(this.Content, this.EntityController, this.Factories);
                            this.fighters.Add(entity);

                            this.fighterNames.Add($"Fighter {this.fighters.Count}");
                        }

                        if (ImGui.Button("Build Space Ship"))
                        {
                            this.BuildSpaceShip();
                        }

                        if (ImGui.Button("Move"))
                        {
                            var entity = this.fighters[this.selectedFighter];
                            var pose = this.Containers.Get<ComponentContainer<Pose>>().Get(entity);
                            var maneuvers = new Queue<IManeuver>();
                            ManeuverPlanner.PlanMoveTo(maneuvers, pose.Position, pose.Yaw, pose.Pitch, this.targetPose.Position, this.linearAcceleration, this.angularAcceleration);
                            this.Factories.Get<FlightPlanFactory>().Construct(entity, maneuvers);
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

        private void BuildSpaceShip()
        {
            var command = new BuildSpaceShipCommand
            {
                Position = Vector3.Zero,
                Scale = Vector3.One
            };

            var entity = command.Execute(this.Content, this.EntityController, this.Factories);
            this.fighters.Add(entity);

            this.fighterNames.Add($"Fighter {this.fighters.Count}");
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

