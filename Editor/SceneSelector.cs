using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Configuration;
using MiniEngine.Scenes;
using MiniEngine.Systems;

namespace MiniEngine
{
    public sealed class SceneSelector
    {
        private readonly EntityManager EntityManager;

        public SceneSelector(ContentManager content, Injector injector)
        {
            this.Scenes = injector.ResolveAll<IScene>()
                              .ToList()
                              .AsReadOnly();

            this.EntityManager = injector.Resolve<EntityManager>();

            foreach (var scene in this.Scenes)
            {
                scene.LoadContent(content);
            }
        }

        public IReadOnlyList<IScene> Scenes { get; private set; }
        public IScene CurrentScene { get; private set; }

        public void SwitchScenes(IScene scene)
        {
            this.EntityManager.Controller.DestroyAllEntities();
            this.CurrentScene = scene;
            this.CurrentScene.Set();
        }
    }
}
