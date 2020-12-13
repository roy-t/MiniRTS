using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Editor.Controllers;
using MiniEngine.Editor.Scenes;
using MiniEngine.Graphics;
using MiniEngine.Gui;

namespace MiniEngine.Editor.Workspaces
{
    [Service]
    public sealed class WorkspaceManager
    {
        private record WorkspaceBinding(string Key, IWorkspace Workspace);

        private readonly SceneManager SceneManager;
        private readonly ImGuiRenderer Gui;
        private readonly EditorStateSerializer Serializer;
        private readonly FrameService FrameService;
        private readonly KeyboardController Keyboard;
        private readonly EditorState State;
        private readonly List<WorkspaceBinding> Workspaces;

        private WorkspaceBinding workspace;

        public WorkspaceManager(IEnumerable<IWorkspace> workspaces, SceneManager sceneManager, ImGuiRenderer gui, EditorStateSerializer serializer, FrameService frameService, KeyboardController keyboard)
        {
            this.SceneManager = sceneManager;
            this.Gui = gui;
            this.Serializer = serializer;
            this.FrameService = frameService;
            this.Keyboard = keyboard;
            this.State = this.Serializer.Deserialize();
            this.Workspaces = workspaces.Select(w => new WorkspaceBinding(w.GetKey(), w)).ToList();

            this.FrameService.CamereComponent.Camera.Move(this.State.CameraPosition, this.State.CameraForward);

            this.workspace = this.Workspaces.FirstOrDefault(w => w.Key == this.State.CurrentWorkspace)
                ?? this.Workspaces.First();
        }

        public void Render(GameTime gameTime)
        {
            if (this.Keyboard.Held(Keys.LeftControl) && this.Keyboard.Released(Keys.Tab))
            {
                this.workspace = this.Workspaces.Next(this.workspace);
            }

            this.Gui.BeforeLayout(gameTime);
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);

            this.RenderMainMenu();
            this.RenderWindows();

            this.Gui.AfterLayout();
        }

        private void RenderMainMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
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

                this.SceneManager.RenderMainMenuItems();
                this.workspace.Workspace.RenderMainMenuItems();

                ImGui.EndMainMenuBar();
            }
        }

        public void RenderWindows()
            => this.workspace.Workspace.RenderWindows();

        public void Save()
        {
            var state = this.State with
            {
                CameraPosition = this.FrameService.CamereComponent.Camera.Position,
                CameraForward = this.FrameService.CamereComponent.Camera.Forward,
                CurrentWorkspace = this.workspace.Key
            };
            this.Serializer.Serialize(state);
        }
    }
}
