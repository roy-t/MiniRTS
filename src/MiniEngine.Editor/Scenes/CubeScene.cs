using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Lighting;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class CubeScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly SkyboxSceneService Skybox;
        private readonly GeneratedAssets GeneratedAssets;
        private readonly GeometryFactory Geometry;
        private readonly LightFactory Lights;

        public CubeScene(GraphicsDevice device, SkyboxSceneService skybox, GeneratedAssets generatedAssets, GeometryFactory geometry, LightFactory lights)
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
            var cube = CubeGenerator.Generate(this.Device);
            var cc = new GeometryModel(cube, new Material(GeneratedAssets.AlbedoPixel(Color.Magenta), GeneratedAssets.NormalPixel(), GeneratedAssets.MetalicnessPixel(0), GeneratedAssets.RoughnessPixel(0), GeneratedAssets.AmbientOcclussionPixel(1)));
            (var geometry, var geometryTransform, var geometryBounds) = this.Geometry.Create(cc);
            //var sponza = content.Load<GeometryModel>("cube/cube");
            //(var geometry, var geometryTransform, var geometryBounds) = this.Geometry.Create(sponza);

            geometryTransform.SetScale(1.0f);

            (var light, var lightTransform) = this.Lights.CreatePointLight();
            light.Strength = 100.0f;
            light.Color = Color.White;

            lightTransform.MoveTo(Vector3.One * 10);
        }
    }
}
