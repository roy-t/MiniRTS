using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Generic
{
    [Service]
    public sealed class PointTool : ATool<Point>
    {
        public override string Name => "Point";

        public override bool HeaderValue(ref Point value, ToolState tool)
        {
            ImGui.Text($"{value}");
            return false;
        }

        public override bool Details(ref Point value, ToolState tool)
        {
            bool slide(ref int x) => ImGui.DragInt(ToolUtils.NoLabel, ref x, 1, (int)tool.X, (int)tool.Y);

            var aspectRatio = value.X / (float)value.Y;
            var changed = ToolUtils.DetailsRow("X", ref value.X, slide);
            changed |= ToolUtils.DetailsRow("Y", ref value.Y, slide);

            return changed;
        }

        public override ToolState Configure(ToolState tool)
        {
            var min = (int)tool.X;
            var max = (int)tool.Y;
            ImGui.InputInt("Min", ref min);
            ImGui.InputInt("Min", ref max);

            tool.X = min;
            tool.Y = max;

            return tool;
        }
    }
}
