using System;
using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class CameraEditor : AEditor<ICamera>
    {
        private readonly FrameService FrameService;
        private readonly Vector3Editor Vector3Editor;

        public CameraEditor(FrameService frameService, Vector3Editor vector3Editor)
        {
            this.FrameService = frameService;
            this.Vector3Editor = vector3Editor;
        }

        public override bool Draw(string name, Func<ICamera> get, Action<ICamera> set)
        {
            var camera = get();

            var changed = false;
            changed |= this.Vector3Editor.Draw($"{name}.Position", () => camera.Position, v => camera.Move(v, camera.Forward));
            changed |= this.Vector3Editor.Draw($"{name}.Forward", () => camera.Forward, v => camera.Move(camera.Position, v));

            if (ImGui.Button("Align to view"))
            {
                var frameCamera = this.FrameService.CamereComponent.Camera;
                camera.Move(frameCamera.Position, frameCamera.Forward);
            }

            return changed;
        }
    }
}
