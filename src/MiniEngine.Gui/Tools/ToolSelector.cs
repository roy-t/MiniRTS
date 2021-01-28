using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
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
            ImGui.PushID(property.Id);

            var state = this.LinkedTools.Get(property);
            var tool = this.GetTool<T>(state);
            value = tool.Select(value, property, state);

            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Button, Color.Black.ToVector4());
            if (ImGui.SmallButton("\u00BB"))
            {
                this.Activate(property);
                this.changingState = state;
            }
            ImGui.PopStyleColor();

            if (this.IsActive(property))
            {
                this.ChangeTool<T>(property);
            }

            ImGui.PopID();

            return value;
        }

        private ATool<T> GetTool<T>(ToolState tool)
        {
            var type = typeof(T);
            if (this.AvailableTools.TryGetValue(type, out var typedTools))
            {
                for (var i = 0; i < typedTools.Tools.Length; i++)
                {
                    if (typedTools.Tools[i].Name == tool.Name)
                    {
                        return (ATool<T>)typedTools.Tools[i];
                    }
                }

                return (ATool<T>)typedTools.Tools[0];
            }

            return new FallbackTool<T>();
        }

        private void ChangeTool<T>(Property property)
        {
            ImGui.OpenPopup("Tool Picker");
            if (ImGui.BeginPopup("Tool Picker"))
            {
                var tool = this.GetTool<T>(this.changingState);
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
