using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Editor.Scenes;
using MiniEngine.Graphics;
using MiniEngine.Gui;
using MiniEngine.Gui.Windows;
using Serilog;

namespace MiniEngine.Editor.Windows
{
    [Service]
    public sealed class MainWindow
    {
        private record EditorState(List<string> OpenWindows, string Scene, Vector3 Position, Vector3 Forward, Dictionary<string, string> KeyValues);

        private readonly ImGuiRenderer Gui;
        private readonly SceneManager SceneManager;
        private readonly FrameService FrameService;
        private readonly WindowService WindowService;
        private readonly IReadOnlyList<IWindow> Windows;
        private readonly Dictionary<string, bool> OpenWindows;
        private readonly PersistentState<EditorState> State;

        private bool showDemoWindow;

        public MainWindow(ILogger logger, ImGuiRenderer gui, SceneManager sceneManager, FrameService frameService, WindowService windowService, IEnumerable<IWindow> windows)
        {
            this.Gui = gui;
            this.SceneManager = sceneManager;
            this.FrameService = frameService;
            this.WindowService = windowService;
            this.Windows = windows.ToList();
            this.OpenWindows = new Dictionary<string, bool>();

            foreach (var window in this.Windows)
            {
                this.OpenWindows[window.Name] = false;
            }

            this.State = new PersistentState<EditorState>(logger, "EditorWindow.json");
            this.Deserialize();

            this.WindowService.OpenWindowEvent += (o, e) => this.OpenWindows[e.Name] = true;
        }

        public void Render(GameTime gameTime)
        {
            this.Gui.BeforeLayout(gameTime);
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport(), ImGuiDockNodeFlags.PassthruCentralNode);

            this.RenderMainMenu();
            this.RenderWindows();

            this.Gui.AfterLayout();
        }

        public void Save() => this.Serialize();

        private void RenderMainMenu()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Windows"))
                {
                    for (var i = 0; i < this.Windows.Count; i++)
                    {
                        var window = this.Windows[i];
                        if (ImGui.MenuItem(window.Name, "", this.OpenWindows[window.Name]))
                        {
                            this.OpenWindows[window.Name] = !this.OpenWindows[window.Name];
                        }
                    }

                    if (ImGui.MenuItem("DemoWindow", "", this.showDemoWindow))
                    {
                        this.showDemoWindow = !this.showDemoWindow;
                    }

                    ImGui.EndMenu();
                }

                this.SceneManager.RenderMainMenuItems();
                ImGui.EndMainMenuBar();
            }
        }

        private void RenderWindows()
        {
            for (var i = 0; i < this.Windows.Count; i++)
            {
                var window = this.Windows[i];
                if (this.OpenWindows[window.Name])
                {
                    var open = true;
                    if (!window.AllowTransparency)
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 1.0f);
                        ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.Black.ToVector4());
                        ImGui.PushStyleColor(ImGuiCol.WindowBg, Color.Black.ToVector4());
                    }
                    if (ImGui.Begin(window.Name, ref open))
                    {
                        window.RenderContents();
                        ImGui.End();

                        this.OpenWindows[window.Name] = open;
                    }
                    if (!window.AllowTransparency)
                    {
                        ImGui.PopStyleVar();
                        ImGui.PopStyleColor(2);
                    }
                }
            }

            if (this.showDemoWindow)
            {
                ImGui.ShowDemoWindow(ref this.showDemoWindow);
            }
        }

        private void Serialize()
        {
            var keyValues = new Dictionary<string, string>();
            foreach (var window in this.Windows)
            {
                window.Save(keyValues);
            }

            var openWindows = this.OpenWindows.Where(kv => kv.Value).Select(kv => kv.Key).ToList();
            var state = new EditorState
            (
                openWindows,
                this.SceneManager.CurrentScene,
                this.FrameService.CameraComponent.Camera.Position,
                this.FrameService.CameraComponent.Camera.Forward,
                keyValues
            );
            this.State.Save(state);
        }

        private void Deserialize()
        {
            var state = this.State.Load();
            if (state != null)
            {
                foreach (var open in state.OpenWindows)
                {
                    this.OpenWindows[open] = true;
                }

                this.SceneManager.SetScene(state.Scene);
                var camera = this.FrameService.CameraComponent.Camera;
                camera.Transform.MoveTo(state.Position);
                camera.Transform.FaceTargetConstrained(state.Position + state.Forward, Vector3.Up);
                camera.Update();

                if (state.KeyValues != null)
                {
                    foreach (var window in this.Windows)
                    {
                        window.Load(state.KeyValues);
                    }
                }
            }
        }
    }
}
