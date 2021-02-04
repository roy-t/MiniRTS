using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Generic
{
    [Service]
    public sealed class ShortStringTool : ATool<string>
    {
        public override string Name => "Short String";

        public override bool HeaderValue(ref string value, ToolState tool)
        {
            ImGui.Text(value);
            return false;
        }
    }
}
