using ImGuiNET;

namespace MiniEngine.UI
{
    public sealed class FileMenu
    {
        private readonly GameLoop GameLoop;

        public FileMenu(UIState uiState, GameLoop gameLoop)
        {
            this.GameLoop = gameLoop;
            this.State = uiState.EditorState;
        }

        public EditorState State { get; }

        public void Render()
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.BeginMenu("Scenes"))
                {
                    foreach (var scene in this.GameLoop.Scenes)
                    {
                        if (ImGui.MenuItem(scene.Name, null, scene == this.GameLoop.CurrentScene))
                        {
                            this.GameLoop.SwitchScenes(scene);
                        }
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Hide GUI", "F12")) { this.State.ShowGui = false; }
                if (ImGui.MenuItem("Quit")) { this.GameLoop.Exit(); }
                ImGui.EndMenu();
            }
        }
    }
}
