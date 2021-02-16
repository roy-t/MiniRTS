using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.ContentPipeline.Packs
{
    public class TexturePack
    {
        public TexturePack(string name)
        {
            this.Name = name;
            this.Names = new List<string>();
            this.Textures = new List<Texture2D>();
        }

        public string Name { get; }

        public List<string> Names { get; }

        public List<Texture2D> Textures { get; }
    }
}
