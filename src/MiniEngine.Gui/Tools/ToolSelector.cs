using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools
{
    [Service]
    public sealed class ToolSelector
    {
        private record TypedTools(Type Type, string[] Names, ITool[] Tools);

        private readonly ToolLinker LinkedTools;
        private readonly ObjectTemplater Templater;
        private readonly Dictionary<Type, TypedTools> AvailableTools;

        private readonly MethodInfo SelectMethod;

        public ToolSelector(ToolLinker linkedTools, ObjectTemplater templater, IEnumerable<ITool> tools)
        {
            this.LinkedTools = linkedTools;
            this.Templater = templater;
            this.AvailableTools = new Dictionary<Type, TypedTools>();
            this.SelectMethod = typeof(ToolSelector).GetMethods().First(m => m.Name.Contains(nameof(Select)) && m.GetParameters().Length == 2);

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

        public bool Select(Type type, ref object? value, Property property)
        {
            var parameters = new[] { value, property };
            var method = this.SelectMethod.MakeGenericMethod(type);
            var changed = method.Invoke(this, parameters) is true;

            value = parameters[0];
            return changed;
        }

        public bool Select<T>(ref T value, Property property)
        {
            // TODO: pass if a field is readonly, then just show textual value
            BeginTable(property);

            var toolState = this.LinkedTools.Get(property);
            var tool = this.GetBestTool<T>(toolState);

            bool showDetails;
            var changed = Header(ref value, out showDetails, property, tool, toolState);
            if (showDetails)
            {
                if (value != null)
                {
                    changed = tool.Details(ref value, toolState);
                }
                this.ToolRow(property, toolState, tool, value);
                ImGui.TreePop();
            }

            if (this.IsActive(property))
            {
                this.ChangeTool<T>(property);
            }

            EndTable();
            return changed;
        }

        private void ToolRow<T>(Property property, ToolState state, ATool<T> tool, T value)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.Text($"Type: {value?.GetType().Name ?? typeof(T).Name}, Tool: {tool.Name}");
            ImGui.NextColumn();

            if (!IsSpecialTool(tool))
            {
                if (ImGui.Button("change tool"))
                {
                    this.Activate(property);
                    this.changingState = state;
                }
            }

            ImGui.NextColumn();
        }

        private bool IsSpecialTool<T>(ATool<T> tool) => tool is ComplexObjectTool<T> || tool is EnumerableTool<T>;

        private static bool Header<T>(ref T value, out bool open, Property property, ATool<T> tool, ToolState toolState)
        {
            // Header
            ImGui.AlignTextToFramePadding();
            open = ImGui.TreeNode(property.Name);

            ImGui.NextColumn();
            ImGui.AlignTextToFramePadding();
            var changed = false;
            if (value == null)
            {
                ImGui.Text("null");
            }
            else
            {
                changed = tool.HeaderValue(ref value, toolState);
            }

            ImGui.NextColumn();
            return changed;
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

            if (typeof(T).IsAssignableTo(typeof(IEnumerable)))
            {
                return new EnumerableTool<T>(this);
            }

            return new ComplexObjectTool<T>(this.Templater, this);
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
