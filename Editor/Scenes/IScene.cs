using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public interface IScene
    {
        string Name { get; }
        void LoadContent(ContentManager content);
        void Set();
        void Update(PerspectiveCamera camera, Seconds elapsed);

        void RenderUI();

        TextureCube Skybox { get; }
    }
}