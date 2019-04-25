using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems.Annotations;

namespace MiniEngine.Pipeline.Debug
{
    public sealed class IconLibrary
    {
        private readonly Dictionary<IconType, Texture2D> Icons;
        private readonly Texture2D DefaultTexture;

        public IconLibrary(ContentManager content, GraphicsDevice device)
        {
            this.Icons = new Dictionary<IconType, Texture2D>
            {
                { IconType.Camera, content.Load<Texture2D>("Icons/Camera") },
                { IconType.LookAt, content.Load<Texture2D>("Icons/LookAt") }
            };

            this.DefaultTexture = new Texture2D(device, 1, 1);
            this.DefaultTexture.SetData(new Color[] { Color.Magenta });
        }

        public Texture2D GetIcon(IconType icon)
        {
            if(this.Icons.TryGetValue(icon, out var texture))
            {
                return texture;
            }

            return this.DefaultTexture;
        }
    }
}
