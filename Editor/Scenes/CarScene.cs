using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Pipeline.Debug.Components;
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
        private CarAnimation carAnimation;
        private AModel indicator;
        private DumbFollowLogic followLogic;
        private Path path;

        private DebugLine pathLine;
        private DebugLine originalLine;
        private System.Numerics.Vector2 endPosition;

        public CarScene(SceneBuilder sceneBuilder)
        {
            this.SceneBuilder = sceneBuilder;
            this.MovementLogic = new DumbMovementLogic();
            this.endPosition = new System.Numerics.Vector2(10, 7);
        }

        public void LoadContent(ContentManager content)
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Car";

        public TextureCube Skybox { get; private set; }

        public void Set()
        {
            (this.carModel, this.carAnimation) = this.SceneBuilder.BuildCar(new Pose(Vector3.Zero, 0.1f));

            this.indicator = this.SceneBuilder.BuildCube(new Pose(Vector3.Zero, 0.0002f));

            this.SceneBuilder.BuildTerrain(40, 40);
            this.SceneBuilder.BuildSponzaAmbientLight();
            this.SceneBuilder.BuildSponzeSunLight();
            this.Skybox = this.SceneBuilder.SponzaSkybox;
            this.CreatePath();
        }

        private void CreatePath()
        {
            var roughPath = this.MovementLogic.PlanPath(0, 0, (int)this.endPosition.X, (int)this.endPosition.Y);
            var smoothPath = PathInterpolator.Interpolate(roughPath);
            this.path = new Path(smoothPath);
            this.followLogic = new DumbFollowLogic(this.carModel, this.carAnimation, this.path, new MetersPerSecond(0.25f));

            this.pathLine = this.SceneBuilder.CreateDebugLine(smoothPath.Select(x => new Vector3(x.X, 0, x.Y)).ToList(), Color.White);
            this.originalLine = this.SceneBuilder.CreateDebugLine(roughPath.Select(x => new Vector3(x.X, 0, x.Y)).ToList(), Color.LightGray);
        }

        public void RenderUI()
        {
            if (ImGui.BeginMenu("Car Scene"))
            {
                ImGui.SliderFloat2("Target", ref this.endPosition, 0, 39);

                if (ImGui.MenuItem("Move"))
                {
                    this.CreatePath();
                }

                ImGui.EndMenu();
            }
        }

        public void Update(Seconds elapsed)
        {
            this.followLogic.Update(elapsed);

            var scale = Matrix.CreateScale(0.00025f);

            var mat = scale * Matrix.CreateTranslation(this.path.GetPositionAfter(this.followLogic.DistanceCovered + 0.3f));
            this.indicator.Pose = new Pose(mat);
        }
    }
}
