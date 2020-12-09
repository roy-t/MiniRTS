using System;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class CameraEditor : AEditor<ICamera>
    {
        public override bool Draw(string name, Func<ICamera> get, Action<ICamera> set)
        {
            var camera = get();

            var position = camera.Position;
            var forward = camera.Forward;
            if (ImGui.DragFloat3($"{name}.Position", ref position))
            {
                camera.Move(position, forward);
                return true;
            }

            if (ImGui.DragFloat3($"{name}.Forward", ref forward))
            {
                camera.Move(position, forward);
                return true;
            }

            return false;
        }
    }
}
