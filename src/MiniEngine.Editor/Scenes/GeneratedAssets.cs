using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.SceneManagement;

namespace MiniEngine.Editor.Scenes
{
    [Service]
    public sealed class GeneratedAssets
    {
        private GraphicsDevice Device;
        private readonly ContentStack Content;

        public GeneratedAssets(GraphicsDevice device, ContentStack content)
        {
            this.Device = device;
            this.Content = content;
        }

        public Texture2D WhitePixel => this.CreatePixel(Color.White);

        public Texture2D BlackPixel => this.CreatePixel(Color.Black);

        public Texture2D RedPixel => this.CreatePixel(Color.Red);

        public Texture2D GreenPixel => this.CreatePixel(Color.Green);

        public Texture2D BluePixel => this.CreatePixel(Color.Blue);

        public Texture2D MetallicPixel => this.CreatePixel(Color.White);

        public Texture2D PlasticPixel => this.CreatePixel(Color.Black);

        public Texture2D AlbedoPixel(Color color)
            => this.CreatePixel(color);

        public Texture2D NormalPixel()
            => this.NormalPixel(Vector3.UnitZ);

        public Texture2D NormalPixel(Vector3 direction)
            => this.CreatePixel(new Color(Pack(direction)));

        public Texture2D MetalicnessPixel(float metalicness)
            => this.CreatePixel(new Color(metalicness, metalicness, metalicness));

        public Texture2D RoughnessPixel(float roughness)
            => this.CreatePixel(new Color(roughness, roughness, roughness));

        public Texture2D AmbientOcclussionPixel(float ao)
            => this.CreatePixel(new Color(ao, ao, ao));

        private Texture2D CreatePixel(Color color)
        {
            var pixel = new Texture2D(this.Device, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { color });

            this.Content.Link(pixel);

            return pixel;
        }

        private static Vector3 Pack(Vector3 direction) => 0.5f * (Vector3.Normalize(direction) + Vector3.One);

    }
}
