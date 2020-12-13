using Microsoft.Xna.Framework;

namespace MiniEngine.UI.State
{
    public sealed class EditorState
    {
        public EditorState()
        {
            this.CameraPosition = Vector3.Zero;
            this.CameraLookAt = Vector3.Forward;
            this.ShowGui = true;
            this.Scene = string.Empty;
            this.CameraSpeed = 10.0f;
        }

        public bool ShowGui { get; set; }
        public Vector3 CameraPosition { get; set; }
        public Vector3 CameraLookAt { get; set; }
        public string Scene { get; set; }

        public float CameraSpeed { get; set; }
    }
}
