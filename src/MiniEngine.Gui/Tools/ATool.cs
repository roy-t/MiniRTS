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

        public abstract T HeaderValue(T value, ToolState tool);

        public virtual T Details(T value, ToolState tool) => value;

        public virtual ToolState Configure(ToolState tool) => tool;

        protected E DetailsRow<E>(string name, Func<E> selector)
        {
            ImGui.PushID(name.GetHashCode());

            ImGui.AlignTextToFramePadding();
            ImGui.TreeNodeEx(name, RowFlags);
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);

            var value = selector();

            ImGui.NextColumn();
            ImGui.PopID();

            return value;
        }
    }
}
