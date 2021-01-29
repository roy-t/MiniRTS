using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Floats
{
    [Service]
    public sealed class SliderFloatTool : ATool<float>
    {
        public override string Name => "Slider";

        public override float HeaderValue(float value, ToolState tool)
        {
            ImGui.SliderFloat(NoLabel, ref value, tool.X, tool.Y);
            return value;
        }

        public override ToolState Configure(ToolState tool)
        {
            ImGui.InputFloat("Min", ref tool.X);
            ImGui.InputFloat("Max", ref tool.Y);

            return tool;
        }
    }
}
