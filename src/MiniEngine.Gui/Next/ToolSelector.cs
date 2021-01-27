using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Next
{
    [Service]
    public sealed class ToolSelector
    {
        private readonly ToolState State;
        private readonly Dictionary<string, AFloatTool> FloatTools;

        public ToolSelector(ToolState state, IEnumerable<AFloatTool> tools)
        {
            this.State = state;
            this.FloatTools = new Dictionary<string, AFloatTool>();
            foreach (var tool in tools)
            {
                this.FloatTools.Add(tool.Type, tool);
            }
        }

        private IntPtr ActiveProperty = IntPtr.Zero;

        private Tool wipTool;

        public float Select(float value, Property property)
        {
            ImGui.PushID(property.Id);

            var state = this.State.Get(property);
            var tool = this.GetTool(state);
            value = tool.Select(value, property, state);
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Button, Color.Black.ToVector4());
            if (ImGui.SmallButton("\u00BB"))
            {
                this.Activate(property);
                this.wipTool = state;
            }
            ImGui.PopStyleColor();

            if (this.IsActive(property))
            {
                this.ChangeTool(property);
            }

            ImGui.PopID();

            return value;
        }

        private AFloatTool GetTool(Tool tool)
        {
            if (this.FloatTools.TryGetValue(tool.Type, out var floatTool))
            {
                return floatTool;
            }

            return this.FloatTools.Values.First();
        }

        private void ChangeTool(Property property)
        {
            ImGui.OpenPopup("Tool Picker");
            if (ImGui.BeginPopup("Tool Picker"))
            {
                var tool = this.GetTool(this.wipTool);
                var allTools = this.FloatTools.Values.ToList();
                var index = allTools.IndexOf(tool);
                if (ImGui.Combo("Tools", ref index, allTools.Select(s => s.Type).ToArray(), allTools.Count))
                {
                    this.wipTool.Type = allTools[index].Type;
                }

                this.wipTool = tool.Configure(this.wipTool);

                if (ImGui.Button("Save"))
                {
                    this.State.Set(property, this.wipTool);
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
            => this.ActiveProperty = property.Id;

        private bool IsActive(Property property)
            => this.ActiveProperty.Equals(property.Id);

        private void Deactivate()
            => this.ActiveProperty = IntPtr.Zero;
    }
}
