using System;
using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class Vector2Editor : AEditor<Vector2>
    {
        public override bool Draw(string name, Func<Vector2> get, Action<Vector2> set)
        {
            var v = get();
            if (ImGui.DragFloat2(name, ref v))
            {
                set(v);
                return true;
            }
            return false;
        }
    }

    [Service]
    public sealed class Vector3Editor : AEditor<Vector3>
    {
        public override bool Draw(string name, Func<Vector3> get, Action<Vector3> set)
        {
            var v = get();
            if (ImGui.DragFloat3(name, ref v))
            {
                set(v);
                return true;
            }
            return false;
        }
    }

    [Service]
    public sealed class Vector4Editor : AEditor<Vector4>
    {
        public override bool Draw(string name, Func<Vector4> get, Action<Vector4> set)
        {
            var v = get();
            if (ImGui.DragFloat4(name, ref v))
            {
                set(v);
                return true;
            }
            return false;
        }
    }
}
