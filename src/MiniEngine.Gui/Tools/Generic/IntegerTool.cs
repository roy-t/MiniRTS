using System;
using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Generic
{
    [Service]
    public sealed class IntegerTool : ATool<int>
    {
        public override string Name => "Integer";

        public override bool HeaderValue(ref int value, ToolState tool)
            => ImGui.SliderInt(ToolUtils.NoLabel, ref value, (int)tool.X, (int)tool.Y);

        public override ToolState Configure(ToolState tool)
        {
            var min = (int)tool.X;
            var max = (int)tool.Y;

            if (ImGui.InputInt("Min", ref min))
            {
                max = Math.Max(min + 1, max);
            }

            if (ImGui.InputInt("Max", ref max))
            {
                min = Math.Min(min, max - 1);
            }

            tool.X = min;
            tool.Y = max;

            return tool;
        }
    }
}
