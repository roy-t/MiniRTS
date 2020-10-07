using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.PostProcess
{
    internal sealed class FullScreenTriangle
    {
        private readonly short[] Indices;
        private readonly PostProcessVertex[] Vertices;

        public FullScreenTriangle()
        {
            this.Vertices = new[]
            {
                new PostProcessVertex(
                    new Vector3(3, -1, 0),
                    new Vector2(2, 1)),
                new PostProcessVertex(
                    new Vector3(-1, -1, 0),
                    new Vector2(0, 1)),
                new PostProcessVertex(
                    new Vector3(-1, 3, 0),
                    new Vector2(0, -1))
            };

            this.Indices = new short[]
            {
                0,
                1,
                2
            };
        }

        public void Render(GraphicsDevice device)
            => device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.Vertices, 0, 3, this.Indices, 0, 1);
    }
}
