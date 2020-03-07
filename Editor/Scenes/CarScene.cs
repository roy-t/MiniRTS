using System.Collections.Generic;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class CarScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;
        private readonly DumbMovementLogic MovementLogic;

        private AModel carModel;

        public CarScene(SceneBuilder sceneBuilder)
        {
            this.SceneBuilder = sceneBuilder;
            this.MovementLogic = new DumbMovementLogic();
            this.checkpoints = new List<Vector2>();
        }

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Car";



        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.carModel = this.SceneBuilder.BuildCar(new Pose(Vector3.Zero));
            this.SceneBuilder.BuildTerrain(40, 40, new Pose(Vector3.Zero));
            this.SceneBuilder.BuildSponzaAmbientLight();
            this.SceneBuilder.BuildSponzeSunLight();
            this.Skybox = this.SceneBuilder.SponzaSkybox;
        }

        public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation)
            => new Pose(translation, scale, rotY, rotX, rotZ);


        private System.Numerics.Vector2 endPosition;
        private int pathIndex;
        private Seconds aggregator;
        private List<Vector2> checkpoints;

        public void RenderUI()
        {
            if (ImGui.BeginMenu("Car Scene"))
            {
                ImGui.SliderFloat2("Target", ref this.endPosition, 0, 39);

                if (ImGui.MenuItem("Move"))
                {
                    this.pathIndex = 0;
                    this.checkpoints = this.MovementLogic.PlanPath(0, 0, (int)this.endPosition.X, (int)this.endPosition.Y);
                }

                ImGui.EndMenu();
            }
        }

        public void Update(Seconds elapsed)
        {
            const float period = 0.15f;

            if (this.checkpoints.Count > 0)
            {
                this.aggregator += elapsed;
                if (this.aggregator > period)
                {
                    this.aggregator -= period;
                    this.pathIndex = (this.pathIndex + 1) % this.checkpoints.Count;
                    var current = this.checkpoints[this.pathIndex];

                    var pose = new Pose(new Vector3(current.X, 0, current.Y));
                    this.carModel.SetPose(pose);
                }
            }
        }
    }
}
