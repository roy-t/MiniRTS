using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Gui.Editors;
using MiniEngine.Gui.Templates;
using MiniEngine.Systems;

namespace MiniEngine.Gui
{
    [Service]
    public sealed class ComponentEditor
    {
        private readonly Dictionary<Type, IPropertyEditor> PropertyEditors;
        private readonly Dictionary<Type, ComponentTemplate> Templates;

        public ComponentEditor(ImGuiRenderer imGui)
        {
            this.PropertyEditors = new Dictionary<Type, IPropertyEditor>
            {
                { typeof(Matrix), new MatrixEditor() },
                { typeof(Entity), new EntityEditor() },
                { typeof(Material), new MaterialEditor(imGui)}
            };

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
