using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Scenes
{
    public sealed class SkyboxBuilder
    {
        private readonly GraphicsDevice Device;

        public SkyboxBuilder(GraphicsDevice device)
        {
            this.Device = device;
        }

        public TextureCube BuildSkyBox(Texture2D back, Texture2D down, Texture2D front, Texture2D left, Texture2D right, Texture2D up)
        {
            var skybox = new TextureCube(this.Device, back.Width, false, SurfaceFormat.Color);

            this.SetFaceData(skybox, CubeMapFace.NegativeZ, back);
            this.SetFaceData(skybox, CubeMapFace.NegativeY, down);
            this.SetFaceData(skybox, CubeMapFace.PositiveZ, front);
            this.SetFaceData(skybox, CubeMapFace.NegativeX, left);
            this.SetFaceData(skybox, CubeMapFace.PositiveX, right);
            this.SetFaceData(skybox, CubeMapFace.PositiveY, up);

            return skybox;
        }

        public TextureCube BuildSkyBox(Color color)
        {
            var skybox = new TextureCube(this.Device, 1, false, SurfaceFormat.Color);
            skybox.SetData(CubeMapFace.PositiveX, new Color[] { color });
            skybox.SetData(CubeMapFace.NegativeX, new Color[] { color });
            skybox.SetData(CubeMapFace.PositiveY, new Color[] { color });
            skybox.SetData(CubeMapFace.NegativeY, new Color[] { color });
            skybox.SetData(CubeMapFace.PositiveZ, new Color[] { color });
            skybox.SetData(CubeMapFace.NegativeZ, new Color[] { color });

            return skybox;
        }

        private void SetFaceData(TextureCube cube, CubeMapFace face, Texture2D data)
        {
            var colors = new Color[data.Width * data.Height];
            data.GetData(colors);
            cube.SetData(face, colors);
        }
    }
}
