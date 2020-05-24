using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Input;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;
using Roy_T.AStar.Primitives;

namespace MiniEngine.Scenes
{
    public sealed class CarScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;
        private readonly OffsetFactory OffsetFactory;
        private WorldGrid worldGrid;

        private Gimbal gimbal;
        private Accelerometer accelerometer;
        private AdditiveEmitter[] emitters;

        private Pose fighterPose;
        private Pose targetPose;
        float radius = 10.0f;
        float yaw = 0.0f;
        float pitch = 0.0f;

        public CarScene(
            SceneBuilder sceneBuilder,
            EntityController entityController,
            MouseInput mouseInput,
            KeyboardInput keyboardInput,
            OffsetFactory offsetFactory)
        {
            this.SceneBuilder = sceneBuilder;
            this.OffsetFactory = offsetFactory;
        }

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Car";

        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            //this.SceneBuilder.BuildTerrain(new Vector2(40, 40));
            this.SceneBuilder.BuildSponzaAmbientLight();
            this.SceneBuilder.BuildSponzeSunLight();
            this.Skybox = this.SceneBuilder.SponzaSkybox;

            this.worldGrid = new WorldGrid(40, 40, 1, 8, new Vector3(-20, 0, -20));
            this.SceneBuilder.CreateDebugLine(this.CreateGridLines(40, 40), Color.White);

            var (cubePose, _, _) = this.SceneBuilder.BuildCube(Vector3.Zero, 0.005f);

            var (fighterPose, fighterModel, fighterBounds) = this.SceneBuilder.BuildFighter(Vector3.Zero, 1.0f);
            this.fighterPose = fighterPose;
            //var (stickyPose, _, _) = this.SceneBuilder.BuildCube(Vector3.Zero, 0.005f);
            //this.OffsetFactory.Construct(stickyPose.Entity, Vector3.Forward * 5, 0, 0, 0, fighterPose.Entity);

            this.targetPose = cubePose;

            this.gimbal = new Gimbal(fighterPose);

            var parent = this.SceneBuilder.BuildParent("Reaction Control Thruster");

            var fighterToNose = Vector3.Forward * 4;
            var (emmiterLeft, emitterPoseLeft, _) = this.SceneBuilder.BuildRCS(fighterPose.Entity, fighterToNose, MathHelper.PiOver2, 0, 0);
            var (emmiterRight, _, _) = this.SceneBuilder.BuildRCS(fighterPose.Entity, fighterToNose, -MathHelper.PiOver2, 0, 0);
            var (emmiterUp, _, _) = this.SceneBuilder.BuildRCS(fighterPose.Entity, fighterToNose, 0, MathHelper.PiOver2, 0);
            var (emmiterDown, _, _) = this.SceneBuilder.BuildRCS(fighterPose.Entity, fighterToNose, 0, -MathHelper.PiOver2, 0);
            this.emitters = new[] { emmiterLeft, emmiterRight, emmiterUp, emmiterDown };
            this.accelerometer = new Accelerometer(emitterPoseLeft);

            parent.Children.Add(emmiterLeft.Entity);
            parent.Children.Add(emmiterRight.Entity);
            parent.Children.Add(emmiterUp.Entity);
            parent.Children.Add(emmiterDown.Entity);
        }

        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            var position = Vector3.Forward;

            this.targetPose.Position = this.radius * Vector3.TransformNormal(position, Matrix.CreateFromYawPitchRoll(this.yaw, this.pitch, 0.0f));

            this.gimbal.PointAt = this.targetPose.Position;
            this.gimbal.Update(elapsed);

            this.accelerometer.Update(elapsed);

            for (var i = 0; i < this.emitters.Length; i++)
            {
                var emitter = this.emitters[i];
                var dot = Vector3.Dot(emitter.Direction, -Vector3.Normalize(this.accelerometer.Acceleration));
                if (dot > 0.15f) // TODO: should be a bit higher than 0, 45 degrees?
                {
                    emitter.StartVelocity = this.accelerometer.Velocity;
                    emitter.Enabled = true;
                }
                else
                {
                    emitter.Enabled = false;
                }
            }

            //this.fighterPose.Position += Vector3.Forward * elapsed;

            //if (this.accelerometer.Acceleration.LengthSquared() > 0)
            //{
            //    this.emitter.Enabled = true;
            //    this.emitter.StartVelocity = this.accelerometer.Velocity; // does this work?
            //    //var length = this.accelerometer.Acceleration.Length();
            //    //this.emitter.Speed = length * 5;
            //}
            //else
            //{
            //    this.emitter.Enabled = false;
            //}
        }

        public void RenderUI()
        {
            if (ImGui.Begin("Scene"))
            {
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

