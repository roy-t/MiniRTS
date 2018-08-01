using Microsoft.Xna.Framework.Content;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public interface IScene
    {        
        void LoadContent(ContentManager content);
        void Set();
        void Update(Seconds elapsed);
    }
}