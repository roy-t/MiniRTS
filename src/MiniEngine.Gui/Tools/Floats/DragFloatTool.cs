using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Floats
{
    [Service]
    public sealed class DragFloatTool : ATool<float>
    {
        public override string Name => "Drag";

        public override int Priority => 10;

        public override bool HeaderValue(ref float value, ToolState tool)
            => ImGui.DragFloat(ToolUtils.NoLabel, ref value, tool.Z, tool.X, tool.Y);

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
