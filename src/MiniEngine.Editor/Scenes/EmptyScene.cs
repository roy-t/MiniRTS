using MiniEngine.Configuration;
using MiniEngine.SceneManagement;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class EmptyScene : IScene
    {
        public void Load(ContentStack content)
        {
        }
    }
}
