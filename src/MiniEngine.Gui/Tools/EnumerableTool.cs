using System;
using System.Collections;
using ImGuiNET;

namespace MiniEngine.Gui.Tools
{
    public class EnumerableTool<T> : ATool<T>
    {
        private readonly ToolSelector ToolSelector;

        public EnumerableTool(ToolSelector toolSelector)
        {
            this.ToolSelector = toolSelector;
        }

        public override string Name => "Collection";

        public override ToolState Configure(ToolState tool) => tool;

        public override bool HeaderValue(ref T value, ToolState state)
        {
            var collection = AsEnumerable(value);
            ImGui.Text($"[{Count(collection)}]");
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

        public override bool Details(ref T value, ToolState tool)
        {
            var type = value?.GetType() ?? typeof(object);

            var collection = AsEnumerable(value);
            ImGui.TreePush(type.Name);
            var index = 0;
            foreach (var item in collection)
            {
                var itemType = item.GetType();
                var o = item;
                this.ToolSelector.Select(itemType, ref o, new Property(itemType.Name + $"[{index}]", $"{type.Name}.{itemType.Name}"));
                index++;
            }

            ImGui.TreePop();

            return false;
        }

        private static IEnumerable AsEnumerable(T value)
            => value as IEnumerable ?? throw new ArgumentException("Not an IEnumerable", nameof(value));
    }
}
