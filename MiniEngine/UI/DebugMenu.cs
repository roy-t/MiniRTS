using ImGuiNET;
using System;

namespace MiniEngine.UI
{
    public sealed class DebugMenu
    {
        private readonly UIRenderer UIRenderer;        
        private readonly GameLoop GameLoop;

        public DebugMenu(UIState ui, UIRenderer uiRenderer, GameLoop gameLoop)
        {
            this.UIRenderer = uiRenderer;
            this.GameLoop = gameLoop;
            this.State = ui.DebugState;
        }

        public DebugState State { get; }

        public void Render()
        {
            if (ImGui.BeginMenu("Debug"))
            {
                if (ImGui.MenuItem(DebugDisplay.None.ToString(), null, this.State.DebugDisplay == DebugDisplay.None))
                {
                    this.State.DebugDisplay = DebugDisplay.None;
                }

                if (ImGui.BeginMenu(DebugDisplay.Combined.ToString()))
                {
                    var descriptions = this.UIRenderer.RenderTargets;
                    var columns = this.State.Columns;
                    if (ImGui.SliderInt("Columns", ref columns, 1, Math.Max(5, descriptions.Count)))
                    {
                        this.State.Columns = columns;
                    }

                    ImGui.Separator();

                    foreach (var target in descriptions)
                    {
                        var selected = this.State.SelectedRenderTargets.Contains(target.Name);
                        if (ImGui.Checkbox(target.Name, ref selected))
                        {
                            if (selected)
                            {
                                this.State.SelectedRenderTargets.Add(target.Name);
                            }
                            else
                            {
                                this.State.SelectedRenderTargets.Remove(target.Name);

                            }

                            this.State.SelectedRenderTargets.Sort();
                            this.State.DebugDisplay = DebugDisplay.Combined;
                        }
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu(DebugDisplay.Single.ToString()))
                {
                    var descriptions = this.UIRenderer.RenderTargets;
                    foreach (var target in descriptions)
                    {
                        var selected = this.State.SelectedRenderTarget == target.Name;
                        if (ImGui.MenuItem(target.Name, null, selected))
                        {
                            this.State.SelectedRenderTarget = target.Name;
                            this.State.DebugDisplay = DebugDisplay.Single;
                        }
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("Fixed Timestep", null, this.GameLoop.IsFixedTimeStep))
                {
                    this.GameLoop.IsFixedTimeStep = !this.GameLoop.IsFixedTimeStep;
                }

                var showDemo = this.State.ShowDemo;
                if (ImGui.MenuItem("Show Demo Window", null, ref showDemo))
                {
                    this.State.ShowDemo = showDemo;
                }

                ImGui.EndMenu();
            }
        }
    }
}
