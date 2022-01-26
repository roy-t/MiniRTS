using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.ParticipatingMedia;
using MiniEngine.Graphics.Particles;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class PointLightScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly SkyboxSceneService Skybox;
        private readonly GeometryFactory Geometry;
        private readonly LightFactory Lights;
        private readonly ParticipatingMediaFactory ParticipatingMedia;
        private readonly ParticleFactory Particles;

        public PointLightScene(GraphicsDevice device, SkyboxSceneService skybox, GeometryFactory geometry, LightFactory lights, ParticipatingMediaFactory participatingMedia, ParticleFactory particles)
        {
            this.Device = device;
            this.Skybox = skybox;
            this.Geometry = geometry;
            this.Lights = lights;
            this.ParticipatingMedia = participatingMedia;
            this.Particles = particles;
        }

        public void RenderMainMenuItems()
           => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            var sponza = content.Load<GeometryModel>("sponza/sponza");
            (var geometry, var geometryTransform, var geometryBounds) = this.Geometry.Create(sponza);

            geometryTransform.SetScale(0.01f);

            (var light, var lightTransform) = this.Lights.CreatePointLight();
            light.Strength = 100.0f;
            light.Color = Color.White;
        }
    }
}
