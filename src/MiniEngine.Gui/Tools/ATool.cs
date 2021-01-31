using System;
using ImGuiNET;

namespace MiniEngine.Gui.Tools
{
    public abstract class ATool<T> : ITool
    {
        public static readonly string NoLabel = "##value";

        private const ImGuiTreeNodeFlags RowFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;

        public Type TargetType => typeof(T);

        public abstract string Name { get; }

        public virtual int Priority => 0;

        public bool HeaderValue(ref object value, ToolState tool)
        {
            var specific = (T)value;
            var changed = this.HeaderValue(ref specific, tool);
            value = specific!;

            return changed;
        }

        public abstract bool HeaderValue(ref T value, ToolState tool);

        public virtual bool Details(ref T value, ToolState tool) => false;

        public bool Details(ref object value, ToolState tool)
        {
            var specific = (T)value;
            var changed = this.Details(ref specific, tool);
            value = specific!;

            return changed;
        }

        public virtual ToolState Configure(ToolState tool) => tool;

        public delegate bool RowDelegate<TValue>(ref TValue value);

        protected bool DetailsRow<E>(string name, ref E value, RowDelegate<E> selector)
        {
            ImGui.PushID(name.GetHashCode());

            ImGui.AlignTextToFramePadding();
            ImGui.TreeNodeEx(name, RowFlags);
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);

            var changed = selector(ref value);

            ImGui.NextColumn();
            ImGui.PopID();

            return changed;
        }

        protected void TextRow(string name, string value)
        {
            ImGui.AlignTextToFramePadding();
            ImGui.TreeNodeEx(name, RowFlags);
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);

            ImGui.Text(value);

            ImGui.NextColumn();
        }
    }
}
