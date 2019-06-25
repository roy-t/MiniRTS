﻿using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.CutScene;

namespace MiniEngine.UI
{
    public sealed class DebugMenu
    {
        private readonly ImGuiRenderer Renderer;
        private readonly RenderTargetDescriber RenderTargetDescriber;
        private readonly CutsceneSystem CutsceneSystem;
        private readonly Game Game;

        private readonly DebugState State;

        public DebugMenu(ImGuiRenderer renderer, UIState ui, RenderTargetDescriber RenderTargetDescriber, CutsceneSystem cutsceneSystem, Game game)
        {
            this.Renderer = renderer;
            this.RenderTargetDescriber = RenderTargetDescriber;
            this.CutsceneSystem = cutsceneSystem;
            this.Game = game;            
            this.State = ui.DebugState;
        }        

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
                    var descriptions = this.RenderTargetDescriber.RenderTargets;
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
                    var descriptions = this.RenderTargetDescriber.RenderTargets;
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

                var textureContrast = this.State.TextureContrast;
                if(ImGui.SliderFloat("Texture Contrast", ref textureContrast, 1.0f, 100.0f))
                {
                    this.State.TextureContrast = textureContrast;
                    this.Renderer.TextureContrast = textureContrast;
                }

                if (ImGui.MenuItem("Fixed Timestep", null, this.Game.IsFixedTimeStep))
                {
                    this.Game.IsFixedTimeStep = !this.Game.IsFixedTimeStep;
                }

                var showDemo = this.State.ShowDemo;
                if (ImGui.MenuItem("Show Demo Window", null, ref showDemo))
                {
                    this.State.ShowDemo = showDemo;
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