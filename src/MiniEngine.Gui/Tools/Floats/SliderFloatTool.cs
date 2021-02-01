using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Floats
{
    [Service]
    public sealed class SliderFloatTool : ATool<float>
    {
        public override string Name => "Slider";

        public override bool HeaderValue(ref float value, ToolState tool)
            => ImGui.SliderFloat(ToolUtils.NoLabel, ref value, tool.X, tool.Y);

        public override ToolState Configure(ToolState tool)
        {
            ImGui.InputFloat("Min", ref tool.X);
            ImGui.InputFloat("Max", ref tool.Y);

            return tool;
        }
    }
}
