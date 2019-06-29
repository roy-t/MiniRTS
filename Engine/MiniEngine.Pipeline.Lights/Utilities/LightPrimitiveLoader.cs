using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Pipeline.Lights.Utilities
{
    public sealed class LightPrimitiveLoader
    {
        private readonly ContentManager Content;

        public LightPrimitiveLoader(ContentManager content)
        {
            this.Content = content;
        }

        public Model UnitSphere() => this.Content.Load<Model>("Effects/sphere");
    }
}
