using ImGuiNET;

namespace MiniEngine.Gui.Tools
{
    public class FallbackTool<T> : ATool<T>
    {
        public override string Name => "Fallback";

        public override ToolState Configure(ToolState tool) => tool;

        public override T Details(T value, ToolState tool) => value;

        public override T HeaderValue(T value)
        {
            ImGui.Text(value?.ToString() ?? string.Empty);
            return value;
        }
    }
}
