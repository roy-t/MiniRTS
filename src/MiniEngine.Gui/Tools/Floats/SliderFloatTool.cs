using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Floats
{
    [Service]
    public sealed class SliderFloatTool : ATool<float>
    {
        public override string Name => "Slider";

        public override float Select(float value, Property property, ToolState tool)
        {
            ImGui.SliderFloat(property.Name, ref value, tool.X, tool.Y);
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
