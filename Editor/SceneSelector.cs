using System.Collections.Generic;
using MiniEngine.GameLogic;
using MiniEngine.Scenes;
using MiniEngine.Systems;

namespace MiniEngine
{
    public sealed class SceneSelector
    {
        private readonly EntityController EntityController;

        public SceneSelector(IList<IScene> scenes, Content content, EntityController entityController)
        {
            this.Scenes = scenes;
            this.EntityController = entityController;

            foreach (var scene in this.Scenes)
            {
                scene.LoadContent(content);
            }
        }

        public IList<IScene> Scenes { get; }
        public IScene CurrentScene { get; private set; }

        public void SwitchScenes(IScene scene)
        {
            this.EntityController.DestroyAllEntities();
            this.CurrentScene = scene;
            this.CurrentScene.Set();
        }

        public void ResetScene()
        {
            this.EntityController.DestroyAllEntities();
            this.CurrentScene.Set();
        }
    }
}
