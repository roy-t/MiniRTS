using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public interface IScene
    {        
        Color AmbientLight { get; }

        void Draw(IViewPoint viewPoint);
        void Draw(Effect effectOverride, IViewPoint viewPoint);
        void LoadContent(ContentManager content);
        void Set(SystemCollection systems);
        void Update(Seconds elapsed);
    }
}