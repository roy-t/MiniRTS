using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class SceneManager
    {
        private readonly IReadOnlyList<IScene> Scenes;
        private IScene scene;

        public SceneManager(IEnumerable<IScene> scenes)
        {
            this.Scenes = scenes.ToList();
            this.scene = scenes.First();
        }

        public void Update(GameTime gameTime)
            => this.scene.Update(gameTime);

        public void RenderMainMenuItems()
        {
            if (ImGui.BeginMenu("Scene"))
            {
                foreach (var scene in this.Scenes)
                {
                    if (ImGui.MenuItem(scene.GetKey(), "", this.scene == scene))
                    {
                        this.scene = scene;
                    }
                }

                ImGui.EndMenu();
            }

            this.scene.RenderMainMenuItems();
        }
    }
}
