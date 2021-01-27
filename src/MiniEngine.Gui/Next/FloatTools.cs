using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Next
{
    public abstract class AFloatTool
    {
        public abstract string Type { get; }

        public abstract float Select(float value, Property property, Tool tool);

        public abstract Tool Configure(Tool tool);
    }

    [Service]
    public sealed class SliderFloatTool : AFloatTool
    {
        public override string Type => "Slider";

        public override float Select(float value, Property property, Tool tool)
        {
            ImGui.SliderFloat(property.Name, ref value, tool.X, tool.Y);
            return value;
        }

        public override Tool Configure(Tool tool)
        {
            ImGui.InputFloat("Min", ref tool.X);
            ImGui.InputFloat("Max", ref tool.Y);

            return tool;
        }
    }

    [Service]
    public sealed class DragFloatTool : AFloatTool
    {
        public override string Type => "Drag";

        public override float Select(float value, Property property, Tool tool)
        {
            ImGui.DragFloat(property.Name, ref value, tool.Z, tool.X, tool.Y);
            return value;
        }

        public override Tool Configure(Tool tool)
        {
            ImGui.InputFloat("Min", ref tool.X);
            ImGui.InputFloat("Max", ref tool.Y);
            ImGui.InputFloat("Speed", ref tool.Z);

            return tool;
        }
    }
}
