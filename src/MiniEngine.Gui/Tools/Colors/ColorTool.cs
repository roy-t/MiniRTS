using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Colors
{
    [Service]
    public sealed class ColorTool : ATool<Color>
    {
        public override string Name => "Color";

        public override bool HeaderValue(ref Color value, ToolState tool)
        {
            var color = value.ToVector4();
            if (ImGui.ColorEdit4(NoLabel, ref color))
            {
                value = new Color(color);
                return true;
            }

            return false;
        }
    }
}
