using System;
using System.Collections;
using ImGuiNET;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class CollectionEditor : AEditor<ICollection>
    {
        public override bool Draw(string name, Func<ICollection> get, Action<ICollection> set)
        {
            var collection = get();
            ImGui.Text($"{name} {{count: {collection.Count}}}");

            return false;
        }
    }
}
