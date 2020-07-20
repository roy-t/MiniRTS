using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Primitives.Cameras;
using MiniEngine.UI.Input;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public interface IScene
    {
        string Name { get; }
        void LoadContent(Content content);
        void Set();
        void Update(PerspectiveCamera camera, Seconds elapsed);

        void RenderUI();

        void HandleInput(PerspectiveCamera camera, KeyboardInput keyboard, MouseInput mouse);

        TextureCube Skybox { get; }
    }
}