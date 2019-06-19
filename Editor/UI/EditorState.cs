using Microsoft.Xna.Framework;

namespace MiniEngine.UI
{
    public sealed class EditorState
    {
        public EditorState()
        {
            this.CameraPosition = Vector3.Zero;
            this.CameraLookAt = Vector3.Forward;
            this.ShowGui = true;
            this.Scene = string.Empty;
        }

        public bool ShowGui { get; set; }
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraLookAt { get; set; }
        public string Scene { get; set; }
    }
}
