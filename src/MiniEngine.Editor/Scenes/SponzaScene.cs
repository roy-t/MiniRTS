using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Geometry;
using MiniEngine.SceneManagement;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SponzaScene : IScene
    {
        private readonly SkyboxSceneService Skybox;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;

        public SponzaScene(SkyboxSceneService skybox, EntityAdministrator entities, ComponentAdministrator components)
        {
            this.Skybox = skybox;
            this.Entities = entities;
            this.Components = components;
        }

        public void RenderMainMenuItems()
           => this.Skybox.SelectSkybox();

        public void Load(ContentStack content)
        {
            var sponza = content.Load<GeometryModel>("sponza/sponza");
            this.CreateModel(sponza, Matrix.CreateScale(0.05f));
        }

        private void CreateModel(GeometryModel model, Matrix transform)
        {
            var entity = this.Entities.Create();
            this.Components.Add(new GeometryComponent(entity, model));
            this.Components.Add(new TransformComponent(entity, transform));
        }
    }
}
