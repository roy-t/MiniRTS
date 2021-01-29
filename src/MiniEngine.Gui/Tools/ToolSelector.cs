using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools
{
    [Service]
    public sealed class ToolSelector
    {
        private record TypedTools(Type Type, string[] Names, ITool[] Tools);

        private readonly ToolLinker LinkedTools;
        private readonly Dictionary<Type, TypedTools> AvailableTools;

        public ToolSelector(ToolLinker linkedTools, IEnumerable<ITool> tools)
        {
            this.LinkedTools = linkedTools;
            this.AvailableTools = new Dictionary<Type, TypedTools>();

            foreach (var tool in tools)
            {
                if (this.AvailableTools.TryGetValue(tool.TargetType, out var typedTools))
                {
                    this.AvailableTools[tool.TargetType] = typedTools with
                    {
                        Names = typedTools.Names.Append(tool.Name).ToArray(),
                        Tools = typedTools.Tools.Append(tool).ToArray()
                    };
                }
                else
                {
                    this.AvailableTools[tool.TargetType] = new TypedTools(tool.TargetType, new[] { tool.Name }, new[] { tool });
                }
            }
        }

        private IntPtr activeProperty = IntPtr.Zero;
        private ToolState changingState;

        public T Select<T>(T value, Property property)
        {
            BeginTable(property);

            var toolState = this.LinkedTools.Get(property);
            var tool = this.GetBestTool<T>(toolState);

            var showDetails = Header(ref value, property, tool, toolState);

            if (showDetails)
            {
                value = tool.Details(value, toolState);
                this.ToolRow(property, toolState, tool);
                ImGui.TreePop();
            }

            if (this.IsActive(property))
            {
                this.ChangeTool<T>(property);
            }

            EndTable();
            return value;
        }

        private void ToolRow<T>(Property property, ToolState state, ATool<T> tool)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.Text($"Type: {typeof(T).Name}, Tool: {tool.Name}");
            ImGui.NextColumn();

            if (!(tool is FallbackTool<T>))
            {
                if (ImGui.Button("change tool"))
                {
                    this.Activate(property);
                    this.changingState = state;
                }
            }

            ImGui.NextColumn();
        }

        private static bool Header<T>(ref T value, Property property, ATool<T> tool, ToolState toolState)
        {
            // Header
            ImGui.AlignTextToFramePadding();
            var open = ImGui.TreeNode(property.Name);

            ImGui.NextColumn();
            ImGui.AlignTextToFramePadding();
            value = tool.HeaderValue(value, toolState);

            ImGui.NextColumn();
            return open;
        }

        private static void EndTable()
        {
            ImGui.PopID();
            ImGui.Columns(1);
        }

        private static void BeginTable(Property property)
        {
            ImGui.Columns(2, "ToolSelectorColumns");
            ImGui.PushID(property.Id);
            ImGui.Separator();
        }

        private ATool<T> GetBestTool<T>(ToolState toolState)
        {
            var tools = this.GetAllTools<T>();
            if (tools.Length > 0)
            {
                for (var i = 0; i < tools.Length; i++)
                {
                    var tool = tools[i];
                    if (tool.Name == toolState.Name)
                    {
                        return tool;
                    }
                }

                return tools[0];
            }

            return new FallbackTool<T>();
        }

        private ATool<T>[] GetAllTools<T>()
        {
            var type = typeof(T);
            if (this.AvailableTools.TryGetValue(type, out var typedTools))
            {
                return typedTools.Tools.OfType<ATool<T>>().ToArray();
            }

            return Array.Empty<ATool<T>>();
        }

        private void ChangeTool<T>(Property property)
        {
            ImGui.OpenPopup("Tool Picker");
            if (ImGui.BeginPopup("Tool Picker"))
            {
                var tool = this.GetBestTool<T>(this.changingState);
                var allTools = this.AvailableTools[typeof(T)];
                var index = Array.IndexOf(allTools.Tools, tool);
                if (ImGui.Combo("Tools", ref index, allTools.Names, allTools.Names.Length))
                {
                    this.changingState.Name = allTools.Tools[index].Name;
                }

                ImGui.Separator();
                this.changingState = tool.Configure(this.changingState);
                ImGui.Separator();

                if (ImGui.Button("Save"))
                {
                    this.LinkedTools.Link(property, this.changingState);
                    this.Deactivate();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    this.Deactivate();
                }

                ImGui.EndPopup();
            }
        }

        private void Activate(Property property)
            => this.activeProperty = property.Id;

        private bool IsActive(Property property)
            => this.activeProperty.Equals(property.Id);

        private void Deactivate()
            => this.activeProperty = IntPtr.Zero;
    }
}
