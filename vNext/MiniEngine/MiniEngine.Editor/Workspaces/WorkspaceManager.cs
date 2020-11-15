using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Graphics;

namespace MiniEngine.Editor.Workspaces
{
    [Service]
    public sealed class WorkspaceManager
    {
        private readonly EditorStateSerializer Serializer;
        private readonly FrameService FrameService;
        private readonly EditorState State;
        private readonly Dictionary<string, IWorkspace> Workspaces;
        private IWorkspace workspace;

        public WorkspaceManager(IEnumerable<IWorkspace> workspaces, EditorStateSerializer serializer, FrameService frameService)
        {
            this.Serializer = serializer;
            this.FrameService = frameService;
            this.State = this.Serializer.Deserialize();
            this.Workspaces = workspaces.ToDictionary(w => w.GetKey(), w => w);

            this.FrameService.Camera.Move(this.State.CameraPosition, this.State.CameraForward);

            if (this.Workspaces.TryGetValue(this.State.CurrentWorkspace ?? string.Empty, out var workspace))
            {
                this.workspace = workspace;
            }
            else
            {
                this.workspace = this.Workspaces.Values.First();
            }
        }

        public void RenderMainMenuItems()
        {
            if (ImGui.BeginMenu("Workspaces"))
            {
                foreach (var pair in this.Workspaces)
                {
                    if (ImGui.MenuItem(pair.Key, "", this.workspace == pair.Value))
                    {
                        this.workspace = pair.Value;
                    }
                }
                ImGui.EndMenu();
            }

            this.workspace.RenderMainMenuItems();
        }

        public void RenderWindows()
            => this.workspace.RenderWindows();

        public void Save()
        {
            var state = this.State with
            {
                CameraPosition = this.FrameService.Camera.Position,
                CameraForward = this.FrameService.Camera.Forward,
                CurrentWorkspace = this.workspace.GetKey()
            };
            this.Serializer.Serialize(state);
        }
    }
}
