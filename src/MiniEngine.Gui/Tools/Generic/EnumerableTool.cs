using System.Collections;
using ImGuiNET;

namespace MiniEngine.Gui.Tools.Generic
{
    public class EnumerableTool : ATool<IEnumerable>
    {
        private readonly Tool Tool;

        public EnumerableTool(Tool tool)
        {
            this.Tool = tool;
        }

        public override string Name => "Enumerable";

        public override ToolState Configure(ToolState tool) => tool;

        public override bool HeaderValue(ref IEnumerable value, ToolState state)
        {
            ImGui.Text($"[{Count(value)}]");
            return false;
        }

        public override bool Details(ref IEnumerable value, ToolState tool)
        {
            var type = value.GetType();

            ImGui.TreePush(type.Name);
            var index = 0;
            foreach (var item in value)
            {
                var itemType = item.GetType();
                var o = item;
                this.Tool.Change(itemType, ref o, new Property(itemType.Name + $"[{index}]", $"{type.Name}.{itemType.Name}"));
                index++;
            }

            ImGui.TreePop();

            return false;
        }

        private static int Count(IEnumerable enumerable)
        {
            var i = 0;
            var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                i++;
            }
            return i;
        }
    }
}
