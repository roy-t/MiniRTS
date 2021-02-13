using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Floats
{
    [Service]
    public sealed class AngleFloatTool : ATool<float>
    {
        public override string Name => "Angle";

        public override bool HeaderValue(ref float value, ToolState tool)
            => ImGui.SliderFloat(ToolUtils.NoLabel, ref value, -MathHelper.Pi, MathHelper.Pi, "%f rad");
    }
}
