using MiniEngine.Configuration;
using MiniEngine.Graphics;

namespace MiniEngine.Editor.Workspaces
{
    [Service]
    public sealed class ModelViewerWorkspace
    {
        private readonly EditorStateSerializer Serializer;
        private readonly FrameService FrameService;
        private readonly EditorState State;

        public ModelViewerWorkspace(EditorStateSerializer serializer, FrameService frameService)
        {
            this.Serializer = serializer;
            this.FrameService = frameService;
            this.State = this.Serializer.Deserialize();
        }

        public void Save()
        {
            var state = this.State with
            {
                CameraPosition = this.FrameService.Camera.Position,
                CameraForward = this.FrameService.Camera.Forward
            };
            this.Serializer.Serialize(state);
        }
    }
}
