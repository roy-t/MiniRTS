using System;

namespace MiniEngine.Gui.Templates
{
    public sealed class PropertyTemplate
    {
        public readonly string Name;
        public readonly Func<object, object> Getter;
        public readonly Action<object, object?> Setter;
        public readonly IPropertyEditor Editor;

        public PropertyTemplate(string name, Func<object, object> getter, Action<object, object?> setter, IPropertyEditor editor)
        {
            this.Name = name;
            this.Getter = getter;
            this.Setter = setter;
            this.Editor = editor;
        }
    }
}
