using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Geometry
{
    public sealed class Geometry
    {
        public Geometry(VertexBuffer vertexBuffer, IndexBuffer indexBuffer)
        {
            this.VertexBuffer = vertexBuffer;
            this.IndexBuffer = indexBuffer;

            this.Primitives = this.IndexBuffer.IndexCount / 3;
        }

        public VertexBuffer VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }

        public int Primitives { get; }
    }
}
