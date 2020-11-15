using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Editor.Controllers;
using MiniEngine.Graphics;

namespace MiniEngine.Editor.Workspaces
{
    [Service]
    public sealed class WorkspaceManager
    {
        private readonly EditorStateSerializer Serializer;
        private readonly FrameService FrameService;
        private readonly KeyboardController Keyboard;
        private readonly EditorState State;
        private readonly List<WorkspaceBinding> Workspaces;
        private WorkspaceBinding workspace;

        public WorkspaceManager(IEnumerable<IWorkspace> workspaces, EditorStateSerializer serializer, FrameService frameService, KeyboardController keyboard)
        {
            this.Serializer = serializer;
            this.FrameService = frameService;
            this.Keyboard = keyboard;
            this.State = this.Serializer.Deserialize();
            this.Workspaces = workspaces.Select(w => new WorkspaceBinding(w.GetKey(), w)).ToList();

            this.FrameService.Camera.Move(this.State.CameraPosition, this.State.CameraForward);

            this.workspace = this.Workspaces.FirstOrDefault(w => w.Key == this.State.CurrentWorkspace)
                ?? this.Workspaces.First();
        }

        public void RenderMainMenuItems()
        {
            if (this.Keyboard.Held(Keys.LeftControl) && this.Keyboard.Released(Keys.Tab))
            {
                this.workspace = this.Workspaces.Next(this.workspace);
            }

            if (ImGui.BeginMenu($"Workspaces ({this.workspace.Key})"))
            {
                foreach (var workspace in this.Workspaces)
                {
                    if (ImGui.MenuItem(workspace.Key, "", this.workspace == workspace))
                    {
                        this.workspace = workspace;
                    }
                }
                ImGui.EndMenu();
            }

            this.workspace.Workspace.RenderMainMenuItems();
        }

        public void RenderWindows()
            => this.workspace.Workspace.RenderWindows();

        public void Save()
        {
            var state = this.State with
            {
                CameraPosition = this.FrameService.Camera.Position,
                CameraForward = this.FrameService.Camera.Forward,
                CurrentWorkspace = this.workspace.Key
            };
            this.Serializer.Serialize(state);
        }

        private record WorkspaceBinding(string Key, IWorkspace Workspace);
    }
}
