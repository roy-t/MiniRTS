using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.ParticipatingMedia;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class DustScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly GeneratedAssets Assets;
        private readonly SkyboxSceneService Skybox;
        private readonly GeometryFactory Geometry;
        private readonly ParticipatingMediaFactory ParticipatingMedia;

        public DustScene(GraphicsDevice device, GeneratedAssets assets, SkyboxSceneService skybox, GeometryFactory geometry, ParticipatingMediaFactory participatingMedia)
        {
            this.Device = device;
            this.Assets = assets;
            this.Skybox = skybox;
            this.Geometry = geometry;
            this.ParticipatingMedia = participatingMedia;
        }

        public void RenderMainMenuItems()
            => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            this.AddAsteroidField();
            this.AddDust();
            this.AddLargeAsteroid(content);
        }

        private void AddLargeAsteroid(ContentStack content)
        {
            var asteroid = content.Load<GeometryModel>("AsteroidField/Asteroid001");
            (var geometry, var transform, var bounds) = this.Geometry.Create(asteroid);
        }

        private void AddDust()
        {
            (var participatingMedia, var participatingMediaTransform) = this.ParticipatingMedia.Create(this.Device.Viewport.Width, this.Device.Viewport.Height);
            participatingMedia.Strength = 4.0f;
            participatingMedia.Color = new Color(0.1f, 0.1f, 0.1f);
            participatingMediaTransform.SetScale(new Vector3(300.0f, 50.0f, 30.0f));
        }

        private void AddAsteroidField()
        {
            var asteroid = SphereGenerator.Generate(this.Device, 15);
            var material = new Material(this.Assets.AlbedoPixel(Color.Red), this.Assets.NormalPixel(), this.Assets.MetalicnessPixel(0.3f), this.Assets.RoughnessPixel(0.5f), this.Assets.AmbientOcclussionPixel(1.0f));
            var model = new GeometryModel(asteroid, material);

            var random = new Random(255);
            var transforms = new Matrix[1024];
            var i = 0;
            for (var x = -16; x < 16; x++)
            {
                for (var z = -16; z < 16; z++)
                {
                    var y = ((float)random.NextDouble() + 0.5f) * 10.0f;

                    var p = new Vector3(x * 2, y, z * 2);
                    transforms[i++] = Matrix.CreateTranslation(p);
                }
            }
            transforms[0] = Matrix.CreateScale(new Vector3(2.0f, 0.1f, 2.0f)) * Matrix.CreateTranslation(Vector3.Up * 30);

            (var geometry, var transform, var bounds, var instancing) = this.Geometry.Create(model, transforms);
        }
    }
}
