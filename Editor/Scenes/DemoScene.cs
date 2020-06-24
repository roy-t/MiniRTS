using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MiniEngine.CutScene;
using MiniEngine.GameLogic;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class DemoScene : IScene
    {
        private readonly SceneBuilder SceneBuilder;
        private readonly CutsceneSystem CutsceneSystem;
        private Seconds accumulator;
        private AmbientLight ambientLight;
        private Sunlight sunlight;
        private PointLight PointLight;
        private ShadowCastingLight shadowCastingLight;
        private Song song;

        private bool started = false;

        public DemoScene(SceneBuilder sceneBuilder, CutsceneSystem cutsceneSystem)
        {
            this.SceneBuilder = sceneBuilder;
            this.CutsceneSystem = cutsceneSystem;
            this.accumulator = 0.0f;
        }

        public void LoadContent(Content content)
        {
            this.Skybox = content.NullSkybox;
            this.song = content.Song;
        }

        public string Name => "Demo";

        public TextureCube Skybox { get; private set; }

        public void Set() => this.SceneBuilder.BuildSponza(Vector3.Zero, 0.05f);

        public void RenderUI() { }

        private float off2;
        public void Update(PerspectiveCamera camera, Seconds elapsed)
        {
            if (this.started == false)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.I))
                {
                    this.started = true;
                }
                return;
            }

            this.CutsceneSystem.Update(camera, elapsed);

            if (this.accumulator == 0)
            {
                this.ambientLight = this.SceneBuilder.BuildSponzaAmbientLight();
                this.sunlight = this.SceneBuilder.BuildSponzaSunLight();
                this.PointLight = this.SceneBuilder.BuildFirePlace();

                this.SceneBuilder.BuildStainedGlass();
                this.SceneBuilder.BuildBulletHoles();

                this.SceneBuilder.BuildCutScene();


                this.ambientLight.Color = Color.Black;
                this.sunlight.Move(this.sunlight.Position, new Vector3(-10.0f, this.sunlight.LookAt.Y, this.sunlight.LookAt.Z));

                this.ambientLight.Color = Color.Black;
                this.sunlight.Move(this.sunlight.Position, new Vector3(-10.0f, this.sunlight.LookAt.Y, this.sunlight.LookAt.Z));
                MediaPlayer.Play(this.song);
            }

            if (this.accumulator < new Seconds(1))
            {
                this.PointLight.Color = Color.IndianRed * this.accumulator;
            }

            this.accumulator += elapsed;

            var warmup = new Seconds(7);
            if (this.accumulator > warmup)
            {
                var since = this.accumulator - warmup;

                var desiredOff2 = MathHelper.Clamp(since / 1, 0.0f, 9.25f);
                this.off2 = MathHelper.Lerp(this.off2, desiredOff2, 0.05f);
                this.sunlight.Move(this.sunlight.Position, new Vector3(-10.0f + this.off2, this.sunlight.LookAt.Y, this.sunlight.LookAt.Z));

                var off = MathHelper.Clamp(since / 23.0f, 0f, 0.4f);
                this.ambientLight.Color = new Color(off, off, off);
            }

            var secondState = new Seconds(26);
            if (this.accumulator > secondState)
            {
                if (this.shadowCastingLight == null)
                {
                    this.shadowCastingLight = this.SceneBuilder.BuildLionSpotLight();
                }

                var since = this.accumulator - secondState;
                var off = MathHelper.Clamp(since / 2.0f, 0f, 0.8f);
                this.shadowCastingLight.Color = Color.White * off;
            }

            var fadeOut = new Seconds(28);
            if (this.accumulator > fadeOut)
            {
                var left = MathHelper.Clamp((this.accumulator - fadeOut) / 3.0f, 0, 1);
                MediaPlayer.Volume = 1.0f - left;
            }

            Console.WriteLine(this.accumulator);
        }
    }
}
