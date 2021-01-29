using System;
using System.Collections.Generic;
using MiniEngine.Configuration;
using MiniEngine.Gui.Templates;
using MiniEngine.Gui.Tools;
using MiniEngine.Systems;

namespace MiniEngine.Gui
{
    [Service]
    public sealed class ComponentEditor
    {
        private readonly Dictionary<Type, ComponentTemplate> Templates;
        private readonly ToolSelector Tools;

        public ComponentEditor(ToolSelector tools)
        {
            this.Tools = tools;
            this.Templates = new Dictionary<Type, ComponentTemplate>();
        }

        public void DrawComponent(AComponent component)
        {
            var type = component.GetType();
            if (!this.Templates.ContainsKey(type))
            {
                this.Templates.Add(type, new ComponentTemplate(type));
            }

            this.Templates[type].Draw(component, this.Tools);
        }
    }
}
