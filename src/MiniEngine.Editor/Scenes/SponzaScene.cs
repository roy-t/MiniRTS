using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.ParticipatingMedia;
using MiniEngine.Graphics.Particles;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SponzaScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly SkyboxSceneService Skybox;
        private readonly GeometryFactory Geometry;
        private readonly ParticipatingMediaFactory ParticipatingMedia;
        private readonly ParticleFactory Particles;

        public SponzaScene(GraphicsDevice device, SkyboxSceneService skybox, GeometryFactory geometry, ParticipatingMediaFactory participatingMedia, ParticleFactory particles)
        {
            this.Device = device;
            this.Skybox = skybox;
            this.Geometry = geometry;
            this.ParticipatingMedia = participatingMedia;
            this.Particles = particles;
        }

        public void RenderMainMenuItems()
           => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            var sponza = content.Load<GeometryModel>("sponza/sponza");
            (var geometry, var geometryTransform, var geometryBounds) = this.Geometry.Create(sponza);

            geometryTransform.SetScale(0.05f);

            (var particpatingMedia, var participatingMediaTransform) = this.ParticipatingMedia.Create(this.Device.Viewport.Width, this.Device.Viewport.Height);

            particpatingMedia.Strength = 4.0f;
            particpatingMedia.Color = new Color(0.1f, 0.1f, 0.1f);
            participatingMediaTransform.SetScale(new Vector3(200, 150.0f, 120.0f));

            (var particleEmitter, var particleTransform, var particleBounds, var forces) = this.Particles.Create(1024 * 1024);
            particleTransform.MoveTo(new Vector3(-49.0f, 3.0f, 0.0f));
            particleTransform.SetRotation(Quaternion.CreateFromYawPitchRoll(0, MathHelper.PiOver2, 0));
        }
    }
}
