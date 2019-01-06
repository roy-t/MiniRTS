using Microsoft.Xna.Framework.Content;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public interface IScene
    {
        string Name { get; }
        void LoadContent(ContentManager content);
        void Set();
        void Update(Seconds elapsed);
    }
}