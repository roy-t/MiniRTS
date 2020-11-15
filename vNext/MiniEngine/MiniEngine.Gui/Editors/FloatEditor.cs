using System;
using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class FloatEditor : AEditor<float>
    {
        public override void Draw(string name, Func<float> get, Action<float> set)
        {
            var f = get();
            if (ImGui.DragFloat(name, ref f))
            {
                set(f);
            }
        }
    }
}
