using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MiniEngine.ContentPipeline.Models
{
    internal class GeometryContent
    {
        public GeometryContent()
        {
            this.Vertices = new VertexBufferContent();
            this.Indices = new IndexCollection();
        }

        public VertexBufferContent Vertices { get; }
        public IndexCollection Indices { get; }
        public Matrix Offset { get; set; }
        public BoundingSphere Bounds { get; set; }
        public MaterialReferences Material { get; set; }
    }
}
