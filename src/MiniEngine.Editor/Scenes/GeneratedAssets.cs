using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.SceneManagement;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class GeneratedAssets
    {
        public GeneratedAssets(GraphicsDevice device, ContentStack content)
        {
            content.Push("generated assets");

            this.WhitePixel = CreatePixel(device, Color.White, content);
            this.BlackPixel = CreatePixel(device, Color.Black, content);
            this.RedPixel = CreatePixel(device, Color.Red, content);
            this.GreenPixel = CreatePixel(device, Color.Green, content);
            this.BluePixel = CreatePixel(device, Color.Blue, content);
            this.NormalPixel = CreatePixel(device, new Color(0.5f, 0.5f, 1.0f), content);

            this.MetallicPixel = this.WhitePixel;
            this.PlasticPixel = this.BlackPixel;
        }

        public Texture2D WhitePixel { get; }

        public Texture2D BlackPixel { get; }

        public Texture2D RedPixel { get; }

        public Texture2D GreenPixel { get; }

        public Texture2D BluePixel { get; }

        public Texture2D NormalPixel { get; }

        public Texture2D MetallicPixel { get; }

        public Texture2D PlasticPixel { get; }


        private static Texture2D CreatePixel(GraphicsDevice device, Color color, ContentStack content)
        {
            var pixel = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { color });

            content.Link(pixel);

            return pixel;
        }

    }
}
