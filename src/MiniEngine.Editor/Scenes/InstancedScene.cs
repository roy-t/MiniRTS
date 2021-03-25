using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Geometry.Generators;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class InstancedScene : IScene
    {
        private readonly GraphicsDevice Device;
        private readonly GeneratedAssets Assets;
        private readonly SkyboxSceneService Skybox;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public InstancedScene(GraphicsDevice device, GeneratedAssets assets, SkyboxSceneService skybox, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Device = device;
            this.Assets = assets;
            this.Skybox = skybox;
            this.Entities = entities;
            this.Components = components;
        }

        public void RenderMainMenuItems()
            => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            var geometry = SphereGenerator.Generate(this.Device, 15);
            var material = new Material(this.Assets.WhitePixel, this.Assets.NormalPixel(), this.Assets.MetalicnessPixel(0.5f), this.Assets.RoughnessPixel(0.0f), this.Assets.AmbientOcclussionPixel(1.0f));

            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, new GeometryModel(geometry, material)));
            this.Components.Add(new TransformComponent(entity));

            var transforms = new Matrix[1024];
            var i = 0;
            for (var x = -16; x < 16; x++)
            {
                for (var y = -16; y < 16; y++)
                {
                    var p = new Vector3(x * 2, y * 2, 0.0f);
                    transforms[i++] = Matrix.CreateTranslation(p);
                }
            }

            this.Components.Add(InstancingComponent.Create(entity, transforms));
        }
    }
}
