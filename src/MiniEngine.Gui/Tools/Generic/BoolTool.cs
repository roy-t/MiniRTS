using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Generic
{
    [Service]
    public sealed class BoolTool : ATool<bool>
    {
        public override string Name => "Bool";

        public override bool HeaderValue(ref bool value, ToolState tool)
            => ImGui.Checkbox("Enabled", ref value);
    }
}
