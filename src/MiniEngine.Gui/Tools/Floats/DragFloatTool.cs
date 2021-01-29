using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Floats
{
    [Service]
    public sealed class DragFloatTool : ATool<float>
    {
        public override string Name => "Drag";

        public override float HeaderValue(float value, ToolState tool)
        {
            ImGui.DragFloat("##value", ref value, tool.Z, tool.X, tool.Y);
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
