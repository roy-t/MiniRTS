using ImGuiNET;
using Microsoft.Xna.Framework;
using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Gui.Tools.Vectors;

namespace MiniEngine.Gui.Tools.Components
{
    [Service]
    public sealed class CameraEditor : ATool<PerspectiveCamera>
    {
        private readonly FrameService FrameService;

        public CameraEditor(FrameService frameService)
        {
            this.FrameService = frameService;
        }

        public override string Name => "Camera";

        public override bool HeaderValue(ref PerspectiveCamera value, ToolState tool)
        {
            ImGui.Text($"p: {VectorUtils.ToShortString(value.Position)} f: {VectorUtils.ToShortString(value.Forward)}");
            return false;
        }

        public override bool Details(ref PerspectiveCamera value, ToolState tool)
        {
            if (ToolUtils.ButtonRow("Camera", "Align to view"))
            {
                var frameCamera = this.FrameService.CameraComponent.Camera;
                value.MoveTo(frameCamera.Position);
                value.FaceTargetConstrained(frameCamera.Position + frameCamera.Forward, Vector3.Up);
                return true;
            }

            return false;
        }
    }
}
