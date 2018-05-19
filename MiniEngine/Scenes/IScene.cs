using Microsoft.Xna.Framework.Content;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public interface IScene
    {        
        void LoadContent(ContentManager content);
        void Set(SystemCollection systems);
        void Update(Seconds elapsed);
    }
}