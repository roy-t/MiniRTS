using ImGuiNET;

namespace MiniEngine.Gui.Tools
{
    public class FallbackTool<T> : ATool<T>
    {
        public override string Name => "Fallback";

        public override ToolState Configure(ToolState tool) => tool;

        public override bool Details(ref T value, ToolState tool) => false;

        public override bool HeaderValue(ref T value, ToolState state)
        {
            ImGui.Text(value?.ToString() ?? string.Empty);
            return false;
        }
    }
}
