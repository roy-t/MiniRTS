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
        private readonly DumbFollowLogic FollowLogic;

        private AModel carModel;


        public CarScene(SceneBuilder sceneBuilder)
        {
            this.SceneBuilder = sceneBuilder;
            this.MovementLogic = new DumbMovementLogic();
            this.FollowLogic = new DumbFollowLogic();
            this.checkpoints = new List<Vector2>(0);
        }

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Car";



        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.carModel = this.SceneBuilder.BuildCar(new Pose(Vector3.Zero, 1.0f / 10.0f));

            this.SceneBuilder.BuildTerrain(40, 40, new Pose(new Vector3(-20, 0, -20)));
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
                    var path = this.MovementLogic.PlanPath(0, 0, (int)this.endPosition.X, (int)this.endPosition.Y);
                    this.FollowLogic.Start(this.carModel, path, new MetersPerSecond(1.0f));
                }

                ImGui.EndMenu();
            }
        }

        public void Update(Seconds elapsed)
        {
            this.FollowLogic.Update(elapsed);
        }
    }
}
