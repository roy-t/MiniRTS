using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.SceneManagement;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SceneManager
    {
        private readonly IReadOnlyList<IScene> Scenes;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly ContentStack Content;

        private IScene? scene;
        private IScene? nextScene;

        public SceneManager(EntityAdministrator entities, ComponentAdministrator components, ContentStack content, IEnumerable<IScene> scenes)
        {
            this.Scenes = scenes.ToList();
            this.Entities = entities;
            this.Components = components;
            this.Content = content;

            this.nextScene = this.Scenes[0];
        }

        public void Update(GameTime gameTime)
        {
            if (this.nextScene != null)
            {
                if (this.scene != null)
                {
                    this.Content.Pop();
                }

                this.Content.Push("scene");
                this.scene = this.nextScene;
                this.scene.Load(this.Content);

                this.nextScene = null;
            }

            this.scene?.Update(gameTime);
        }

        public void RenderMainMenuItems()
        {
            if (ImGui.BeginMenu("Scene"))
            {
                foreach (var scene in this.Scenes)
                {
                    if (ImGui.MenuItem(scene.GetKey(), "", this.scene == scene))
                    {
                        this.nextScene = scene;
                        this.CleanUp();
                    }
                }

                ImGui.EndMenu();
            }

            this.scene?.RenderMainMenuItems();
        }

        private void CleanUp()
        {
            var entities = this.Entities.GetAllEntities();
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                if (entity.Id > 1)
                {
                    this.Components.MarkForRemoval(entity);
                }
            }
        }
    }
}
