using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.VertexTypes;

namespace MiniEngine.Primitives
{
    public sealed class FullScreenQuad
    {
        private readonly GraphicsDevice Device;
        private readonly short[] Indices;
        private readonly GBufferVertex[] Vertices;

        public FullScreenQuad(GraphicsDevice device)
        {
            this.Device = device;
            this.Vertices = new[]
            {
                new GBufferVertex(
                    new Vector3(1, -1, 0),
                    new Vector2(1, 1)),
                new GBufferVertex(
                    new Vector3(-1, -1, 0),
                    new Vector2(0, 1)),
                new GBufferVertex(
                    new Vector3(-1, 1, 0),
                    new Vector2(0, 0)),
                new GBufferVertex(
                    new Vector3(1, 1, 0),
                    new Vector2(1, 0))
            };

            this.Indices = new short[]
            {
                0,
                1,
                2,
                2,
                3,
                0
            };
        }

        public void SetTextureCoordinates(Vector2 minUv, Vector2 maxUv)
        {
            this.Vertices[0].TextureCoordinate.X = maxUv.X;
            this.Vertices[0].TextureCoordinate.Y = maxUv.Y;

            this.Vertices[1].TextureCoordinate.X = minUv.X;
            this.Vertices[1].TextureCoordinate.Y = maxUv.Y;

            this.Vertices[2].TextureCoordinate.X = minUv.X;
            this.Vertices[2].TextureCoordinate.Y = minUv.Y;

            this.Vertices[3].TextureCoordinate.X = maxUv.X;
            this.Vertices[3].TextureCoordinate.Y = minUv.Y;
        }

        public void Render()
        {
            this.Device.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList,
                this.Vertices,
                0,
                4,
                this.Indices,
                0,
                2);
        }

        public void Render(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3,
            Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3)
        {
            this.Vertices[0].Position = new Vector4(v0, 1);
            this.Vertices[0].TextureCoordinate = uv0;

            this.Vertices[1].Position = new Vector4(v1, 1);
            this.Vertices[1].TextureCoordinate = uv1;

            this.Vertices[2].Position = new Vector4(v2, 1);
            this.Vertices[2].TextureCoordinate = uv2;

            this.Vertices[3].Position = new Vector4(v3, 1);
            this.Vertices[3].TextureCoordinate = uv3;

            this.Render();
        }
    }
}