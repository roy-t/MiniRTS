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

        public ComponentTemplate(Type type, List<IPropertyEditor> editors)
        {
            this.Name = type.Name;
            this.Properties = new List<PropertyTemplate>();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                object getter(object c) => property.GetMethod!.Invoke(c, null)!;
                void setter(object c, object? m) => property.GetSetMethod()!.Invoke(c, new object?[] { m });

                var editor = GetEditor(editors, property);
                if (editor != null)
                {
                    this.Properties.Add(new PropertyTemplate(property.Name, getter, setter, editor));
                }
                else
                {
                    this.Properties.Add(new PropertyTemplate(property.Name, getter, setter, new UnknownPropertyTypeEditor()));
                }
            }
        }

        private static IPropertyEditor? GetEditor(List<IPropertyEditor> editors, PropertyInfo property)
        {
            for (var j = 0; j < editors.Count; j++)
            {
                var editor = editors[j];
                if (editor.TargetType.IsAssignableFrom(property.PropertyType))
                {
                    return editor;
                }
            }

            return null;
        }

        public void Draw(AComponent component)
        {
            if (ImGui.CollapsingHeader(this.Name))
            {
                var changed = false;
                foreach (var property in this.Properties)
                {
                    changed |= property.Editor.Draw(property.Name, property.Getter, property.Setter, component);
                }

                if (changed)
                {
                    component.ChangeState.Change();
                }
            }
        }
    }
}
