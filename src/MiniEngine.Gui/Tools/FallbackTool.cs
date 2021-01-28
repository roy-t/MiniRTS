using ImGuiNET;

namespace MiniEngine.Gui.Tools
{
    public class FallbackTool<T> : ATool<T>
    {
        public override string Name => "Fallback";

        public override ToolState Configure(ToolState tool) => tool;
        public override T Select(T value, Property property, ToolState tool)
        {
            ImGui.Text($"{value}");
            return value;
        }
    }
}
