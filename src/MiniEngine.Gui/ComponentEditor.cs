using MiniEngine.Configuration;
using MiniEngine.Gui.Tools;
using MiniEngine.Systems;

namespace MiniEngine.Gui
{
    [Service]
    public sealed class ComponentEditor
    {
        private readonly ToolSelector Tools;

        public ComponentEditor(ToolSelector tools)
        {
            this.Tools = tools;
        }

        public void DrawComponent(AComponent component)
        {
            var type = component.GetType();
            var o = (object?)component;
            //var changed = this.Tools.Select(type, ref o, new Property(type.Name));
            var changed = this.Tools.Select(ref component, new Property(type.Name));
            if (changed)
            {
                component.ChangeState.Change();
            }
        }
    }
}
