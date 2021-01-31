using System;
using ImGuiNET;

namespace MiniEngine.Gui.Tools
{
    public record ToolResult<T>(bool Changed, T Value);

    public abstract class ATool<T> : ITool
    {
        public delegate bool RowDelegate<TValue>(ref TValue value);

        public static readonly string NoLabel = "##value";

        private const ImGuiTreeNodeFlags RowFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;

        public Type TargetType => typeof(T);

        public abstract string Name { get; }

        public abstract bool HeaderValue(ref T value, ToolState tool);

        public virtual bool Details(ref T value, ToolState tool) => false;

        public virtual ToolState Configure(ToolState tool) => tool;

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
