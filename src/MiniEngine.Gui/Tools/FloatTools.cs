using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools
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

    [Service]
    public sealed class DragFloatTool : ATool<float>
    {
        public override string Name => "Drag";

        public override float Select(float value, Property property, ToolState tool)
        {
            ImGui.DragFloat(property.Name, ref value, tool.Z, tool.X, tool.Y);
            return value;
        }

        public override ToolState Configure(ToolState tool)
        {
            var isFinite = float.IsFinite(tool.X) || float.IsFinite(tool.Y);
            if (ImGui.Checkbox("Finite", ref isFinite))
            {
                if (isFinite)
                {
                    tool.X = 0.0f;
                    tool.Y = 1.0f;
                }
                else
                {
                    tool.X = float.NegativeInfinity;
                    tool.Y = float.PositiveInfinity;
                }
            }

            if (isFinite)
            {
                ImGui.InputFloat("Min", ref tool.X);
                ImGui.InputFloat("Max", ref tool.Y);
            }
            ImGui.InputFloat("Speed", ref tool.Z);

            return tool;
        }
    }
}
