using Microsoft.Xna.Framework.Graphics;
using MiniEngine.GameLogic;
using MiniEngine.Primitives.Cameras;
using MiniEngine.UI.Input;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class EmptyScene : IScene
    {
        public string Name => "Empty Scene";
        public TextureCube Skybox { get; private set; }

        public void HandleInput(PerspectiveCamera camera, KeyboardInput keyboard, MouseInput mouse) { }
        public void LoadContent(Content content)
            => this.Skybox = content.NullSkybox;

        public void RenderUI() { }
        public void Set() { }
        public void Update(PerspectiveCamera camera, Seconds elapsed) { }
    }
}
