using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Systems;

namespace MiniEngine.Editor.Editors
{
    [Service]
    public sealed class ComponentEditor
    {
        private readonly Dictionary<Type, IPropertyEditor> PropertyEditors;
        private readonly Dictionary<Type, WholeComponentEditor> ComponentEditors;

        // TODO: move all this crap to MiniEngine.Gui, auto discover property editors, clean up this class

        public ComponentEditor()
        {
            this.PropertyEditors = new Dictionary<Type, IPropertyEditor>
            {
                { typeof(Matrix), new MatrixEditor() },
                { typeof(Entity), new EEditor() }
            };

            this.ComponentEditors = new Dictionary<Type, WholeComponentEditor>();
        }

        public void DrawComponent(AComponent component)
        {
            var type = component.GetType();
            if (!this.ComponentEditors.ContainsKey(type))
            {
                this.ComponentEditors.Add(type, new WholeComponentEditor(type, this.PropertyEditors));
            }

            this.ComponentEditors[type].Draw(component);
        }
    }

    public sealed class Property
    {
        public readonly string Name;
        public readonly Func<object, object> Getter;
        public readonly Action<object, object?> Setter;
        public readonly IPropertyEditor Editor;

        public Property(string name, Func<object, object> getter, Action<object, object?> setter, IPropertyEditor editor)
        {
            this.Name = name;
            this.Getter = getter;
            this.Setter = setter;
            this.Editor = editor;
        }
    }

    public sealed class WholeComponentEditor
    {
        private readonly List<Property> Properties;

        public WholeComponentEditor(Type type, Dictionary<Type, IPropertyEditor> editors)
        {
            this.Properties = new List<Property>();

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                object gg(object c) => property.GetMethod!.Invoke(c, null)!;
                void ss(object c, object? m) => property.GetSetMethod()!.Invoke(c, new object?[] { m });
                if (editors.TryGetValue(property.PropertyType, out var editor))
                {
                    this.Properties.Add(new Property(property.Name, gg, ss, editor));
                }
                else
                {
                    this.Properties.Add(new Property(property.Name, gg, ss, new UnknownPropertyTypeEditor()));
                }
            }
        }

        public void Draw(AComponent component)
        {
            foreach (var property in this.Properties)
            {
                property.Editor.Draw(property.Name, property.Getter, property.Setter, component);
            }
        }
    }

    public interface IPropertyEditor
    {
        void Draw(string name, Func<object, object> get, Action<object, object?> set, AComponent component);
    }

    public abstract class AEditor<T> : IPropertyEditor
    {
        public void Draw(string name, Func<object, object> get, Action<object, object?> set, AComponent component)
        {
            Func<T> typedGet = () => (T)get(component);
            Action<T> typedSet = it => set(component, it);
            this.Draw(name, typedGet, typedSet);
        }

        public abstract void Draw(string name, Func<T> get, Action<T> set);
    }

    public sealed class MatrixEditor : AEditor<Matrix>
    {
        public override void Draw(string name, Func<Matrix> get, Action<Matrix> set)
        {
            var matrix = get();
            var translation = matrix.Translation;
            if (ImGui.DragFloat3($"{name}.Translation", ref translation))
            {
                matrix.Translation = translation;
                set(matrix);
            }
        }
    }

    public sealed class EEditor : AEditor<Entity>
    {
        public override void Draw(string name, Func<Entity> get, Action<Entity> set)
        {
            // no-op
        }
    }

    public sealed class UnknownPropertyTypeEditor : IPropertyEditor
    {
        public void Draw(string name, Func<object, object> get, Action<object, object?> set, AComponent component)
        {
            var value = get(component);
            ImGui.Text($"{name}: {value} : {value.GetType().Name}");
        }
    }
}
