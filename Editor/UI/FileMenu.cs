using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Primitives.Cameras;
using MiniEngine.UI.State;

namespace MiniEngine.UI
{
    public sealed class FileMenu : IMenu
    {
        private readonly Game Game;
        private readonly SceneSelector SceneSelector;
        

        public FileMenu(Game game, SceneSelector sceneSelector)
        {        
            this.Game = game;
            this.SceneSelector = sceneSelector;
            this.State = new UIState();
        }        

        public UIState State { get; set; }
        public EditorState EditorState => State.EditorState;

        public void Render(PerspectiveCamera camera)
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
                            this.EditorState.Scene = scene.Name;
                        }
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Hide GUI", "F12")) { this.EditorState.ShowGui = false; }
                if (ImGui.MenuItem("Quit")) { this.Game.Exit(); }
                ImGui.EndMenu();
            }
        }
    }
}
