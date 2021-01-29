using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;
using MiniEngine.Gui.Tools;
using MiniEngine.Systems;

namespace MiniEngine.Gui.Templates
{
    public sealed class ComponentTemplate
    {
        private record PropertyTemplate(string Name, Type Type, MethodInfo? Getter, MethodInfo? Setter);

        private readonly List<PropertyTemplate> Properties;
        private readonly string Name;

        public ComponentTemplate(Type type)
        {
            this.Name = type.Name;
            this.Properties = new List<PropertyTemplate>();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                this.Properties.Add(new PropertyTemplate(property.Name, property.PropertyType, property.GetGetMethod(), property.GetSetMethod()));
            }


        }

        public void Draw(AComponent component, ToolSelector tools)
        {
            if (ImGui.CollapsingHeader(this.Name))
            {
                var changed = false;
                foreach (var property in this.Properties)
                {
                    var value = property.Getter?.Invoke(component, null);
                    changed |= tools.Select(property.Type, ref value, new Property(component.GetType().Name, property.Name));

                    if (changed)
                    {
                        property.Setter?.Invoke(component, new[] { value });
                    }
                }

                if (changed)
                {
                    component.ChangeState.Change();
                }
            }
        }
    }
}
