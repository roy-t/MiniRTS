using Microsoft.Xna.Framework;

namespace MiniEngine.Editor.Scenes
{
    public interface IScene
    {
        public string GetKey() => this.GetType().Name;

        public void Load(ContentStack content);

        void Update(GameTime gameTime) { }

        void RenderMainMenuItems() { }
    }
}
