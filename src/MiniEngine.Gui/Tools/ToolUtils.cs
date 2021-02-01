﻿using ImGuiNET;

namespace MiniEngine.Gui.Tools
{
    public static class ToolUtils
    {
        public delegate bool RowDelegate<T>(ref T value);

        public static readonly string NoLabel = "##value";

        private const ImGuiTreeNodeFlags RowFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen | ImGuiTreeNodeFlags.Bullet;

        public static bool DetailsRow<T>(string name, ref T value, RowDelegate<T> selector)
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

        public static bool ButtonRow(string name, string button)
        {
            ImGui.PushID(name.GetHashCode());

            ImGui.AlignTextToFramePadding();
            ImGui.TreeNodeEx(name, RowFlags);
            ImGui.NextColumn();
            ImGui.SetNextItemWidth(-1);

            var clicked = ImGui.Button(button);

            ImGui.NextColumn();
            ImGui.PopID();

            return clicked;
        }

        public static void TextRow(string name, string value)
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
