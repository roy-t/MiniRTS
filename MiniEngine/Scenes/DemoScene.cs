using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Primitives;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class DemoScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;

        private Seconds accumulator;
        private AmbientLight ambientLight;
        private Sunlight sunlight;
        private ShadowCastingLight pilarLight1;
        private ShadowCastingLight pilarLight2;

        public DemoScene(SceneBuilder sceneBuilder)
        {
            this.SceneBuilder = sceneBuilder;
            this.accumulator = 0.0f;
        }

        public void LoadContent(ContentManager content) 
            => this.SceneBuilder.LoadContent(content);

        public string Name => "Demo";

        public void Set()
        {
            this.SceneBuilder.BuildSponza(new Pose(Vector3.Zero, 0.05f));

            this.ambientLight = this.SceneBuilder.BuildSponzaAmbientLight();
            this.sunlight = this.SceneBuilder.BuildSponzeSunLight();

            this.SceneBuilder.BuildStainedGlass();
            this.SceneBuilder.BuildFirePlace();
            this.SceneBuilder.BuildBulletHoles();

            this.SceneBuilder.BuildCutScene();
        }

        public static Pose CreateScaleRotationTranslation(float scale, float rotX, float rotY, float rotZ, Vector3 translation) 
            => new Pose(translation, scale, rotY, rotX, rotZ);

        public void Update(Seconds elapsed)
        {
            if(this.accumulator == 0)
            {
                this.ambientLight.Color = Color.Black;
                this.sunlight.Move(this.sunlight.Position, new Vector3(-10.0f, this.sunlight.LookAt.Y, this.sunlight.LookAt.Z));
            }

            this.accumulator += elapsed;

            var warmup = new Seconds(8);
            if(this.accumulator > warmup)
            {
                var since = this.accumulator - warmup;


                var off2 = MathHelper.Clamp(since / 1, 0.0f, 9.25f);
                this.sunlight.Move(this.sunlight.Position, new Vector3(-10.0f + off2, this.sunlight.LookAt.Y, this.sunlight.LookAt.Z));

                var off = MathHelper.Clamp(since / 23.0f, 0f, 0.4f);
                this.ambientLight.Color = new Color(off, off, off);
            }


            var secondState = new Seconds(15);
            if(this.accumulator > secondState)
            {
                var since = this.accumulator - secondState;
                if (this.pilarLight1 == null)
                {
                    this.pilarLight1 = this.SceneBuilder.BuildPilarLight1();
                    this.pilarLight2 = this.SceneBuilder.BuildPilarLight2();
                }

                var off3 = MathHelper.Clamp(since / 4.0f, 0.0f, 0.8f);
                this.pilarLight1.Color = new Color(off3, off3, off3);
                this.pilarLight2.Color = this.pilarLight1.Color;

            }

        }
    }
}
