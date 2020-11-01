using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Skybox
{
    public static class SkyboxGenerator
    {
        public static SkyboxGeometry Generate(GraphicsDevice device, TextureCube texture, TextureCube environment)
        {
            var vertices = new SkyboxVertex[8];
            var indices = new short[36];

            const float extend = 1.0f;

            vertices[0] = new SkyboxVertex(-extend, extend, extend);  // top NW
            vertices[1] = new SkyboxVertex(extend, extend, extend);   // top NE
            vertices[2] = new SkyboxVertex(extend, extend, -extend);   // top SE
            vertices[3] = new SkyboxVertex(-extend, extend, -extend);   // top SW

            vertices[4] = new SkyboxVertex(-extend, -extend, extend);  // bottom NW
            vertices[5] = new SkyboxVertex(extend, -extend, extend);   // bottom NE
            vertices[6] = new SkyboxVertex(extend, -extend, -extend);   // bottom SE
            vertices[7] = new SkyboxVertex(-extend, -extend, -extend);   // bottom SW

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

            var vertexBuffer = new VertexBuffer(device, SkyboxVertex.Declaration, vertices.Length, BufferUsage.None);
            vertexBuffer.SetData(vertices);

            var indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
            indexBuffer.SetData(indices);

            return new SkyboxGeometry(texture, environment, vertexBuffer, indexBuffer);
        }
    }
}
