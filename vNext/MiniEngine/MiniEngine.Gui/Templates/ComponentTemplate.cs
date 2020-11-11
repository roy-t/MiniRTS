using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;
using MiniEngine.Gui.Editors;
using MiniEngine.Systems;

namespace MiniEngine.Gui.Templates
{
    public sealed class ComponentTemplate
    {
        private readonly List<PropertyTemplate> Properties;
        private readonly string Name;

        public ComponentTemplate(Type type, Dictionary<Type, IPropertyEditor> editors)
        {
            this.Name = type.Name;
            this.Properties = new List<PropertyTemplate>();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                object getter(object c) => property.GetMethod!.Invoke(c, null)!;
                void setter(object c, object? m) => property.GetSetMethod()!.Invoke(c, new object?[] { m });
                if (editors.TryGetValue(property.PropertyType, out var editor))
                {
                    this.Properties.Add(new PropertyTemplate(property.Name, getter, setter, editor));
                }
                else
                {
                    this.Properties.Add(new PropertyTemplate(property.Name, getter, setter, new UnknownPropertyTypeEditor()));
                }
            }
        }

        public void Draw(AComponent component)
        {
            if (ImGui.CollapsingHeader(this.Name))
            {
                foreach (var property in this.Properties)
                {
                    property.Editor.Draw(property.Name, property.Getter, property.Setter, component);
                }
            }
        }
    }
}
