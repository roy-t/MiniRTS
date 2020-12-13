using System;
using ImGuiNET;
using MiniEngine.Systems;

namespace MiniEngine.Gui.Editors
{
    public sealed class UnknownPropertyTypeEditor : IPropertyEditor
    {
        public bool Draw(string name, Func<object, object> get, Action<object, object?> set, AComponent component)
        {
            var value = get(component);
            ImGui.Text($"{name}: {value}");

            return false;
        }

        public Type TargetType => typeof(object);
    }
}
