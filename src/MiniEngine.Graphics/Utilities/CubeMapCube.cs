using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Utilities
{
    public sealed class CubeMapCube : IDisposable
    {
        public CubeMapCube(GraphicsDevice device)
        {
            var vertices = new VertexPosition[8];
            var indices = new short[36];

            const float extend = 1.0f;

            vertices[0] = new VertexPosition(new Vector3(-extend, extend, extend));  // top NW
            vertices[1] = new VertexPosition(new Vector3(extend, extend, extend));   // top NE
            vertices[2] = new VertexPosition(new Vector3(extend, extend, -extend));   // top SE
            vertices[3] = new VertexPosition(new Vector3(-extend, extend, -extend));   // top SW
            vertices[4] = new VertexPosition(new Vector3(-extend, -extend, extend));  // bottom NW
            vertices[5] = new VertexPosition(new Vector3(extend, -extend, extend));   // bottom NE
            vertices[6] = new VertexPosition(new Vector3(extend, -extend, -extend));   // bottom SE
            vertices[7] = new VertexPosition(new Vector3(-extend, -extend, -extend));   // bottom SW

            // top
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 2;

            indices[3] = 2;
            indices[4] = 3;
            indices[5] = 0;

            // north
            indices[6] = 1;
            indices[7] = 0;
            indices[8] = 4;

            indices[9] = 4;
            indices[10] = 5;
            indices[11] = 1;

            // east
            indices[12] = 2;
            indices[13] = 1;
            indices[14] = 5;

            indices[15] = 5;
            indices[16] = 6;
            indices[17] = 2;

            // south
            indices[18] = 3;
            indices[19] = 2;
            indices[20] = 6;

            indices[21] = 6;
            indices[22] = 7;
            indices[23] = 3;

            // west
            indices[24] = 0;
            indices[25] = 3;
            indices[26] = 7;

            indices[27] = 7;
            indices[28] = 4;
            indices[29] = 0;

            // bottom
            indices[30] = 5;
            indices[31] = 4;
            indices[32] = 7;

            indices[33] = 7;
            indices[34] = 6;
            indices[35] = 5;

            this.VertexBuffer = new VertexBuffer(device, VertexPosition.VertexDeclaration, vertices.Length, BufferUsage.None);
            this.VertexBuffer.SetData(vertices);

            this.IndexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            this.IndexBuffer.SetData(indices);

            this.Primitives = indices.Length / 3;
        }

        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }
        public int Primitives { get; }

        public void Dispose()
        {
            this.VertexBuffer.Dispose();
            this.IndexBuffer.Dispose();
        }
    }
}
