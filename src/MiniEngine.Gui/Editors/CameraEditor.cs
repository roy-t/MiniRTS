using System;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;

namespace MiniEngine.Gui.Editors
{
    [Service]
    public sealed class CameraEditor : AEditor<ICamera>
    {
        private readonly Vector3Editor Vector3Editor;

        public CameraEditor(Vector3Editor vector3Editor)
        {
            this.Vector3Editor = vector3Editor;
        }

        public override bool Draw(string name, Func<ICamera> get, Action<ICamera> set)
        {
            var camera = get();

            var changed = false;
            changed |= this.Vector3Editor.Draw($"{name}.Position", () => camera.Position, v => camera.Move(v, camera.Forward));
            changed |= this.Vector3Editor.Draw($"{name}.Forward", () => camera.Forward, v => camera.Move(camera.Position, v));
            return changed;
        }
    }
}
