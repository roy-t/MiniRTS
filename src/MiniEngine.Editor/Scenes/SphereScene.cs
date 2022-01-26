using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Lighting;
using System;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SphereScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly SkyboxSceneService Skybox;
        private readonly GeneratedAssets GeneratedAssets;
        private readonly GeometryFactory Geometry;
        private readonly LightFactory Lights;

        public SphereScene(GraphicsDevice device, SkyboxSceneService skybox, GeneratedAssets generatedAssets, GeometryFactory geometry, LightFactory lights)
        {
            this.Device = device;
            this.Skybox = skybox;
            this.GeneratedAssets = generatedAssets;
            this.Geometry = geometry;
            this.Lights = lights;
        }

        public void RenderMainMenuItems()
            => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            var red = new Texture2D(this.Device, 1, 1);
            red.SetData(new Color[] { Color.White });
            content.Link(red);

            var white = new Texture2D(this.Device, 1, 1);
            white.SetData(new Color[] { Color.White });
            content.Link(white);

            var black = new Texture2D(this.Device, 1, 1);
            black.SetData(new Color[] { Color.Black });
            content.Link(black);

            var normal = new Texture2D(this.Device, 1, 1);
            normal.SetData(new Color[] { new Color(0.5f, 0.5f, 1.0f) });
            content.Link(normal);

            var blue = content.Load<Texture2D>("Textures/Blue");
            var bumps = content.Load<Texture2D>("Textures/Bricks_Normal");

            var rows = 7;
            var columns = 7;
            var spacing = 2.5f;
            var geometry = SphereGenerator.Generate(this.Device, 15);
            for (var row = 0; row < rows; row++)
            {
                var metalicness = row / (float)rows;

                var metalicnessTexture = new Texture2D(this.Device, 1, 1);
                metalicnessTexture.SetData(new Color[] { new Color(Vector3.One * metalicness) });
                content.Link(metalicnessTexture);

                for (var col = 0; col < columns; col++)
                {
                    var roughness = Math.Clamp(col / (float)columns, 0.05f, 1.0f);
                    var roughnessTexture = new Texture2D(this.Device, 1, 1);
                    roughnessTexture.SetData(new Color[] { new Color(Vector3.One * roughness) });
                    content.Link(roughnessTexture);

                    var material = new Material(red, normal, metalicnessTexture, roughnessTexture, white);

                    var position = new Vector3((col - (columns / 2.0f)) * spacing, (row - (rows / 2.0f)) * spacing, 0.0f);
                    this.CreateSphere(geometry, material, position, Vector3.One);
                }
            }

            var backgroundGeometry = CubeGenerator.Generate(this.Device);
            this.CreateSphere(backgroundGeometry, new Material(bumps, GeneratedAssets.NormalPixel(), black, white, white), Vector3.Forward * 20, new Vector3(200, 200, 1));

            this.CreateLight(new Vector3(-10, 10, 10), Color.Red, 30.0f);
            this.CreateLight(new Vector3(10, 10, 10), Color.Blue, 30.0f);
            this.CreateLight(new Vector3(-10, -10, 10), Color.Green, 30.0f);
            this.CreateLight(new Vector3(10, -10, 10), Color.White, 30.0f);

            this.CreateSpotLight(new Vector3(0, 0, 10), Vector3.Forward, 1500.0f);
        }

        private void CreateSphere(GeometryData geometry, Material material, Vector3 position, Vector3 scale)
        {
            var mesh = new GeometryMesh(geometry, material, Matrix.Identity);
            var model = new GeometryModel();
            model.Add(mesh);

            (var sphereGeometry, var sphereTransform, var sphereBounds) = this.Geometry.Create(model);
            sphereTransform.MoveTo(position);
            sphereTransform.SetScale(scale);
        }

        private void CreateLight(Vector3 position, Color color, float strength)
        {
            (var pointLight, var transform) = this.Lights.CreatePointLight();
            pointLight.Color = color;
            pointLight.Strength = strength;
            transform.MoveTo(position);
        }

        private void CreateSpotLight(Vector3 position, Vector3 forward, float strength)
        {
            (var spotLight, var shadowMap, var viewPoint) = this.Lights.CreateSpotLight(1024);
            spotLight.Strength = strength;
            spotLight.Color = Color.Yellow;
            viewPoint.Camera.MoveTo(position);
            viewPoint.Camera.FaceTargetConstrained(position + forward, Vector3.Up);
        }
    }
}
