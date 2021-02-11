﻿using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Gui;
using MiniEngine.Gui.Windows;
using Serilog;

namespace MiniEngine.Editor.Windows
{
    [Service]
    public sealed class EditorWindow
    {
        private record EditorState(List<string> OpenWindows);

        private readonly ImGuiRenderer Gui;
        private readonly IReadOnlyList<IWindow> Windows;
        private readonly Dictionary<string, bool> OpenWindows;
        private readonly PersistentState<EditorState> State;

        public EditorWindow(ILogger logger, ImGuiRenderer gui, IEnumerable<IWindow> windows)
        {
            this.Gui = gui;
            this.Windows = windows.ToList();
            this.OpenWindows = new Dictionary<string, bool>();

            foreach (var window in this.Windows)
            {
                this.OpenWindows[window.Name] = false;
            }

            this.State = new PersistentState<EditorState>(logger, "EditorWindow.json");
            this.Deserialize();
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

                    ImGui.EndMenu();
                }

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
        }

        private void Serialize()
        {
            var state = new EditorState(this.OpenWindows.Where(kv => kv.Value).Select(kv => kv.Key).ToList());
            this.State.Serialize(state);
        }

        private void Deserialize()
        {
            var state = this.State.Deserialize();
            if (state != null)
            {
                foreach (var open in state.OpenWindows)
                {
                    this.OpenWindows[open] = true;
                }
            }
        }
    }
}
