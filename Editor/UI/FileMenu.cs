using ImGuiNET;
using Microsoft.Xna.Framework;

namespace MiniEngine.UI
{
    public sealed class FileMenu
    {
        private readonly Game Game;
        private readonly SceneSelector SceneSelector;
        private readonly EditorState State;

        public FileMenu(UIState uiState, Game game, SceneSelector sceneSelector)
        {
            this.State = uiState.EditorState;
            this.Game = game;
            this.SceneSelector = sceneSelector;            
        }        

        public void Render()
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.BeginMenu("Scenes"))
                {
                    foreach (var scene in this.SceneSelector.Scenes)
                    {
                        if (ImGui.MenuItem(scene.Name, null, scene == this.SceneSelector.CurrentScene))
                        {
                            this.SceneSelector.SwitchScenes(scene);
                            this.State.Scene = scene.Name;
                        }
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Hide GUI", "F12")) { this.State.ShowGui = false; }
                if (ImGui.MenuItem("Quit")) { this.Game.Exit(); }
                ImGui.EndMenu();
            }
        }
    }
}
