using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools
{
    [Service]
    public sealed class Tool
    {
        private readonly ToolLinker LinkedTools;
        private readonly ObjectTemplater Templater;
        private readonly ToolSelector ToolSelector;

        private readonly MethodInfo SelectMethod;

        private IntPtr activeProperty = IntPtr.Zero;
        private ToolState changingState;

        public Tool(ToolLinker linkedTools, ObjectTemplater templater, IEnumerable<ITool> tools)
        {
            this.LinkedTools = linkedTools;
            this.Templater = templater;
            this.ToolSelector = new ToolSelector(this, templater, tools);

            this.SelectMethod = typeof(Tool).GetMethods().First(m => m.Name.Contains(nameof(Change)) && m.GetParameters().Length == 2);
        }

        public static void BeginTable(string name)
            => ImGui.Columns(2, name);

        public static void EndTable()
        {
            ImGui.Columns(1);
            ImGui.Separator();
        }

        public bool Change(Type type, ref object value, Property property)
        {
            ImGui.PushID(property.Id);
            ImGui.Separator();

            var toolState = this.LinkedTools.Get(property);
            var tool = this.ToolSelector.GetBestTool(type, toolState);

            var changed = Header(ref value, out var showDetails, property, tool, toolState);
            if (showDetails)
            {
                changed |= tool.Details(ref value, toolState);
                this.ToolRow(property, toolState, tool, type);
                ImGui.TreePop();
            }

            if (this.IsActive(property))
            {
                this.ChangeTool(type, property);
            }
            ImGui.PopID();
            return changed;
        }

        public bool Change<T>(ref T value, Property property)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            var generic = (object)value;
            var changed = this.Change(typeof(T), ref generic, property);

            value = (T)generic;
            return changed;
        }

        private static bool Header(ref object value, out bool open, Property property, ITool tool, ToolState toolState)
        {
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

        private void ChangeTool(Type type, Property property)
        {
            ImGui.OpenPopup("Tool Picker");
            if (ImGui.BeginPopup("Tool Picker"))
            {
                var allTools = this.ToolSelector.GetAllTools(type);
                var bestTool = this.ToolSelector.GetBestTool(type, this.changingState);

                var index = Array.IndexOf(allTools.Tools, bestTool);
                if (ImGui.Combo("Tools", ref index, allTools.Names, allTools.Names.Length))
                {
                    this.changingState.Name = allTools.Tools[index].Name;
                }

                ImGui.Separator();
                this.changingState = bestTool.Configure(this.changingState);
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

        private void ToolRow(Property property, ToolState state, ITool tool, Type type)
        {
            if (ToolUtils.ButtonRow($"Type: {type.Name}, Tool: {tool.Name}", "change tool"))
            {
                this.Activate(property);
                this.changingState = state;
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
