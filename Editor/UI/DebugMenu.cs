using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.CutScene;
using MiniEngine.Primitives.Cameras;
using MiniEngine.UI.State;
using MiniEngine.UI.Utilities;

namespace MiniEngine.UI
{
    public sealed class DebugMenu : IMenu
    {
        private readonly ImGuiRenderer Renderer;
        private readonly RenderTargetDescriber RenderTargetDescriber;
        private readonly CutsceneSystem CutsceneSystem;
        private readonly Game Game;


        public DebugMenu(ImGuiRenderer renderer, RenderTargetDescriber RenderTargetDescriber, CutsceneSystem cutsceneSystem, Game game)
        {
            this.Renderer = renderer;
            this.RenderTargetDescriber = RenderTargetDescriber;
            this.CutsceneSystem = cutsceneSystem;
            this.Game = game;
            this.State = new UIState();
        }        

        public UIState State { get; set; }
        public DebugState DebugState => State.DebugState;

        public void Render(PerspectiveCamera camera)
        {
            if (ImGui.BeginMenu("Debug"))
            {
                if (ImGui.MenuItem(DebugDisplay.None.ToString(), null, this.DebugState.DebugDisplay == DebugDisplay.None))
                {
                    this.DebugState.DebugDisplay = DebugDisplay.None;
                }

                if (ImGui.BeginMenu(DebugDisplay.Combined.ToString()))
                {
                    var descriptions = this.RenderTargetDescriber.RenderTargets;
                    var columns = this.DebugState.Columns;
                    if (ImGui.SliderInt("Columns", ref columns, 1, Math.Max(5, descriptions.Count)))
                    {
                        this.DebugState.Columns = columns;
                    }

                    ImGui.Separator();

                    foreach (var target in descriptions)
                    {
                        var selected = this.DebugState.SelectedRenderTargets.Contains(target.Name);
                        if (ImGui.Checkbox(target.Name, ref selected))
                        {
                            if (selected)
                            {
                                this.DebugState.SelectedRenderTargets.Add(target.Name);
                            }
                            else
                            {
                                this.DebugState.SelectedRenderTargets.Remove(target.Name);

                            }

                            this.DebugState.SelectedRenderTargets.Sort();
                            this.DebugState.DebugDisplay = DebugDisplay.Combined;
                        }
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu(DebugDisplay.Single.ToString()))
                {
                    var descriptions = this.RenderTargetDescriber.RenderTargets;
                    foreach (var target in descriptions)
                    {
                        var selected = this.DebugState.SelectedRenderTarget == target.Name;
                        if (ImGui.MenuItem(target.Name, null, selected))
                        {
                            this.DebugState.SelectedRenderTarget = target.Name;
                            this.DebugState.DebugDisplay = DebugDisplay.Single;
                        }
                    }
                    ImGui.EndMenu();
                }

                var textureContrast = this.DebugState.TextureContrast;
                if(ImGui.SliderFloat("Texture Contrast", ref textureContrast, 1.0f, 100.0f))
                {
                    this.DebugState.TextureContrast = textureContrast;
                    this.Renderer.TextureContrast = textureContrast;
                }

                if (ImGui.MenuItem("Fixed Timestep", null, this.Game.IsFixedTimeStep))
                {
                    this.Game.IsFixedTimeStep = !this.Game.IsFixedTimeStep;
                }

                var showDemo = this.DebugState.ShowDemo;
                if (ImGui.MenuItem("Show Demo Window", null, ref showDemo))
                {
                    this.DebugState.ShowDemo = showDemo;
                }

                if (ImGui.MenuItem("Start Cutscene", null))
                {
                    this.CutsceneSystem.Start();
                }

                ImGui.EndMenu();
            }
        }
    }
}
