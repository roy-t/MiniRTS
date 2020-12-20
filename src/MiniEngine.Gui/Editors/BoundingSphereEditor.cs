using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class BoundingSphereEditor : AEditor<BoundingSphere>
    {
        public override bool Draw(string name, Func<BoundingSphere> get, Action<BoundingSphere> set)
        {
            var bounds = get();
            ImGui.Text($"Bounds: {bounds.Center}, {bounds.Radius}");
            return false;
        }
    }
}
