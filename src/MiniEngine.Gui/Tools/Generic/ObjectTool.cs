﻿using System;
using ImGuiNET;

namespace MiniEngine.Gui.Tools.Generic
{
    public class ObjectTool : ITool
    {
        public static bool ShowReadOnlyProperties = true;

        private readonly ObjectTemplater Templater;
        private readonly Tool Tool;

        public ObjectTool(ObjectTemplater templater, Tool tool)
        {
            this.Templater = templater;
            this.Tool = tool;
        }

        public string Name => "Object";

        public Type TargetType => typeof(object);

        public int Priority => 0;

        public ToolState Configure(ToolState tool) => tool;

        public bool HeaderValue(ref object value, ToolState state)
        {
            var header = string.Empty;
            var type = value.GetType();
            var template = this.Templater.GetTemplate(type);

            if (template.ValueHeader)
            {
                header = value?.ToString() ?? string.Empty;
            }

            ImGui.Text(header);
            return false;
        }

        public bool Details(ref object value, ToolState tool, Property propertyPath)
        {
            var changed = false;
            var type = value.GetType();
            var template = this.Templater.GetTemplate(type);

            for (var i = 0; i < template.Properties.Count; i++)
            {
                var property = template.Properties[i];
                var propertyValue = property.Getter.Invoke(value, null);
                if (propertyValue != null)
                {
                    if (property.Setter == null && !ShowReadOnlyProperties)
                    {
                        continue;
                    }

                    if (property.Setter == null)
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 0.4f);
                    }
                    else
                    {
                        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, 1.0f);
                    }

                    if (this.Tool.Change(property.Type, ref propertyValue, propertyPath.Dot(property.Name)))
                    {
                        property.Setter?.Invoke(value, new[] { propertyValue });
                        changed = true;
                    }

                    ImGui.PopStyleVar();
                }
            }
            return changed;
        }
    }
}
