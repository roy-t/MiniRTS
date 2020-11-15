using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class ColorEditor : AEditor<Color>
    {
        public override void Draw(string name, Func<Color> get, Action<Color> set)
        {
            var color = get().ToVector4();
            if (ImGui.ColorPicker4(name, ref color))
            {
                set(new Color(color));
            }
        }
    }
}
