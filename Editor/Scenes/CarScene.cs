using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class CarScene : IScene
    {
        private readonly PerspectiveCamera camera;
        private readonly SceneBuilder SceneBuilder;
        private readonly DumbMovementLogic MovementLogic;
        private readonly DumbFollowLogic FollowLogic;

        private Car car;
        private AModel carModel;
        private CarAnimation carAnimation;
        private AModel indicator;

        private DebugLine pathLine;
        private System.Numerics.Vector2 endPosition;

        public CarScene(PerspectiveCamera camera, SceneBuilder sceneBuilder)
        {
            this.camera = camera;
            this.SceneBuilder = sceneBuilder;
            this.MovementLogic = new DumbMovementLogic();
            this.FollowLogic = new DumbFollowLogic();
            this.endPosition = new System.Numerics.Vector2(10, 7);
        }

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Car";



        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            this.carModel = this.SceneBuilder.BuildCar(new Pose(Vector3.Zero, 0.1f));
            //this.carModel.Origin = new Vector3(-0.075f, 0, 0); // TODO: should we take scaling into account in offset?

            this.carAnimation = new CarAnimation();
            this.carModel.Animation = this.carAnimation;
            this.carAnimation.SetTarget(this.carModel);

            this.car = new Car(this.carModel);

            this.indicator = this.SceneBuilder.BuildCube(new Pose(Vector3.Zero, 0.0002f));

            this.SceneBuilder.BuildTerrain(40, 40, new Pose(new Vector3(-20, 0, -20)));
            this.SceneBuilder.BuildSponzaAmbientLight();
            this.SceneBuilder.BuildSponzeSunLight();
            this.Skybox = this.SceneBuilder.SponzaSkybox;

            var path = this.MovementLogic.PlanPath(0, 0, (int)this.endPosition.X, (int)this.endPosition.Y);
            this.FollowLogic.Start(this.carModel, path, new MetersPerSecond(0.1f));

            this.pathLine = this.SceneBuilder.CreateDebugLine(path.Select(x => new Vector3(x.X, 0, x.Y)).ToList(), Color.White);
        }



        public void RenderUI()
        {
            if (ImGui.BeginMenu("Car Scene"))
            {
                ImGui.SliderFloat2("Target", ref this.endPosition, 0, 39);

                if (ImGui.MenuItem("Move"))
                {
                    var path = this.MovementLogic.PlanPath(0, 0, (int)this.endPosition.X, (int)this.endPosition.Y);
                    this.pathLine.Positions = path.Select(x => new Vector3(x.X, 0, x.Y)).ToList();
                    this.FollowLogic.Start(this.carModel, path, new MetersPerSecond(0.1f));
                }

                ImGui.EndMenu();
            }
        }

        public void Update(Seconds elapsed)
        {
            this.carAnimation.Update(elapsed);
            this.FollowLogic.Update(elapsed);

            var scale = Matrix.CreateScale(0.00025f);

            //var mat = scale * Matrix.CreateTranslation(car.GetWheelPosition(WheelPosition.FrontLeft));
            var mat = scale * Matrix.CreateTranslation(this.FollowLogic.GetLookAt(0.3f));
            this.indicator.Pose = new Pose(mat);
        }
    }
}
