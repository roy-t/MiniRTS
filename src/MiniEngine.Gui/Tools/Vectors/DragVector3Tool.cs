using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Gui.Tools.Vectors
{
    [Service]
    public sealed class DragVector3Tool : ATool<Vector3>
    {
        public override string Name => "Drag";

        public override bool HeaderValue(ref Vector3 value, ToolState tool)
            => ImGui.DragFloat3(NoLabel, ref value);
    }
}
