using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Floats
{
    [Service]
    public sealed class AngleFloatTool : ATool<float>
    {
        public override string Name => "Angle";

        public override float Select(float value, Property property, ToolState tool)
        {
            ImGui.SliderFloat(property.Name, ref value, -MathHelper.PiOver2, MathHelper.PiOver2, "%f rad");
            return value;
        }
    }
}
