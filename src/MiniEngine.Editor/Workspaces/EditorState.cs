using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Editor.Workspaces
{
    public sealed record EditorState
    (
        Entity Entity,
        Vector3 CameraPosition,
        Vector3 CameraForward,
        string? CurrentWorkspace,
        string Scene
    );
}
