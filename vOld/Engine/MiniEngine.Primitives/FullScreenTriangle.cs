using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Primitives
{
    public sealed class FullScreenTriangle
    {
        private readonly short[] Indices;
        private readonly VertexPositionTexture[] Vertices;

        public FullScreenTriangle()
        {
            this.Vertices = new[]
            {
                new VertexPositionTexture(
                    new Vector3(3, -1, 0),
                    new Vector2(2, 1)),
                new VertexPositionTexture(
                    new Vector3(-1, -1, 0),
                    new Vector2(0, 1)),
                new VertexPositionTexture(
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

        /// <summary>
        /// Renders a triangle that covers the entire screen (when using device coordinates)
        /// </summary>
        public void Render(GraphicsDevice device)
            => device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.Vertices, 0, 3, this.Indices, 0, 1);
    }
}