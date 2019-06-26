using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Scenes;
using MiniEngine.Systems;

namespace MiniEngine
{
    public sealed class SceneSelector
    {
        private readonly EntityManager EntityManager;

        public SceneSelector(IList<IScene> scenes, ContentManager content, EntityManager entityManager)
        {
            this.Scenes = scenes;
            this.EntityManager = entityManager;

            foreach (var scene in this.Scenes)
            {
                scene.LoadContent(content);
            }
        }

        public IList<IScene> Scenes { get; }
        public IScene CurrentScene { get; private set; }

        public void SwitchScenes(IScene scene)
        {
            this.EntityManager.Controller.DestroyAllEntities();
            this.CurrentScene = scene;
            this.CurrentScene.Set();
        }
    }
}
