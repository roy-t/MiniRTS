using Microsoft.Xna.Framework;

namespace MiniEngine.Editor
{
    internal interface IGameLoop
    {
        void Draw(GameTime gameTime);
        void Stop();
        bool Update(GameTime gameTime);
    }
}