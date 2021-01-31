using ImGuiNET;

namespace MiniEngine.Gui.Tools
{
    public class ComplexObjectTool<T> : ATool<T>
    {
        private readonly ObjectTemplater Templater;
        private readonly ToolSelector ToolSelector;

        public ComplexObjectTool(ObjectTemplater templater, ToolSelector toolSelector)
        {
            this.Templater = templater;
            this.ToolSelector = toolSelector;
        }

        public override string Name => "ComplexObject";

        public override ToolState Configure(ToolState tool) => tool;

        public override bool HeaderValue(ref T value, ToolState state)
        {
            var header = string.Empty;
            var type = value?.GetType() ?? typeof(T);
            var template = this.Templater.GetTemplate(type);

            if (template.ValueHeader)
            {
                header = value?.ToString() ?? string.Empty;
            }

            ImGui.Text(header);
            return false;
        }

        public override bool Details(ref T value, ToolState tool)
        {
            var changed = false;
            var type = value?.GetType() ?? typeof(T);
            var template = this.Templater.GetTemplate(type);

            for (var i = 0; i < template.Properties.Count; i++)
            {
                var property = template.Properties[i];
                var propertyValue = property.Getter?.Invoke(value, null);
                changed |= this.ToolSelector.Select(property.Type, ref propertyValue, new Property(property.Name));

                if (changed)
                {
                    property.Setter?.Invoke(value, new[] { propertyValue });
                }
            }
            return changed;
        }
    }
}
