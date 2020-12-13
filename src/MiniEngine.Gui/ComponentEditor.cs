using System;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Configuration;
using MiniEngine.Gui.Templates;
using MiniEngine.Systems;

namespace MiniEngine.Gui
{
    [Service]
    public sealed class ComponentEditor
    {
        private readonly List<IPropertyEditor> PropertyEditors;
        private readonly Dictionary<Type, ComponentTemplate> Templates;

        public ComponentEditor(IEnumerable<IPropertyEditor> editors)
        {
            this.PropertyEditors = editors.ToList();
            this.Templates = new Dictionary<Type, ComponentTemplate>();
        }

        public void DrawComponent(AComponent component)
        {
            var type = component.GetType();
            if (!this.Templates.ContainsKey(type))
            {
                this.Templates.Add(type, new ComponentTemplate(type, this.PropertyEditors));
            }

            this.Templates[type].Draw(component);
        }
    }
}
