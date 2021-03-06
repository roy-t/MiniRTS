using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SceneManager
    {
        private readonly IReadOnlyList<IScene> Scenes;
        private readonly FrameService FrameService;
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly ContentStack Content;

        private IScene? scene;
        private IScene? nextScene;

        public SceneManager(FrameService frameService, EntityAdministrator entities, ComponentAdministrator components, ContentStack content, IEnumerable<IScene> scenes)
        {
            this.Scenes = scenes.ToList();
            this.FrameService = frameService;
            this.Entities = entities;
            this.Components = components;
            this.Content = content;

            this.nextScene = this.Scenes[0];
        }

        public string CurrentScene => this.scene?.GetKey() ?? string.Empty;

        public void SetScene(string key)
        {
            var scene = this.Scenes.FirstOrDefault(s => s.GetKey().Equals(key, StringComparison.OrdinalIgnoreCase));
            if (scene != null)
            {
                this.SetScene(scene);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (this.nextScene != null)
            {
                if (this.scene != null)
                {
                    this.Content.Pop();
                }

                this.Content.Push($"scene - {this.nextScene.GetKey()}");
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
                        SetScene(scene);
                    }
                }

                ImGui.EndMenu();
            }

            this.scene?.RenderMainMenuItems();
        }

        private void SetScene(IScene scene)
        {
            this.nextScene = scene;
            this.CleanUp();
        }

        private void CleanUp()
        {
            var entities = this.Entities.GetAllEntities();
            for (var i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                this.Components.MarkForRemoval(entity);
                this.Entities.Remove(entity);
            }

            this.FrameService.Reset();
        }
    }
}
