using ImGuiNET;
using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Camera;
using MiniEngine.Gui.Tools.Vectors;

namespace MiniEngine.Gui.Tools.Components
{
    [Service]
    public sealed class CameraEditor : ATool<ICamera>
    {
        private readonly FrameService FrameService;

        public CameraEditor(FrameService frameService)
        {
            this.FrameService = frameService;
        }

        public override string Name => "Camera";

        public override bool HeaderValue(ref ICamera value, ToolState tool)
        {
            ImGui.Text($"p: {VectorUtils.ToShortString(value.Position)} f: {VectorUtils.ToShortString(value.Forward)}");
            return false;
        }

        public override bool Details(ref ICamera value, ToolState tool)
        {
            if (ToolUtils.ButtonRow("Camera", "Align to view"))
            {
                var frameCamera = this.FrameService.CamereComponent.Camera;
                value.Move(frameCamera.Position, frameCamera.Forward);
                return true;
            }

            return false;
        }
    }
}
