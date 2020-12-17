using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MiniEngine.ContentPipeline.Models
{
    public sealed class PoseContent
    {
        public PoseContent()
        {
            this.Vertices = new VertexBufferContent();
            this.Indices = new IndexCollection();

            this.Offset = Matrix.Identity;
            this.Bounds = new BoundingSphere(Vector3.Zero, 0.0f);
        }

        public VertexBufferContent Vertices { get; }
        public IndexCollection Indices { get; }
        public Matrix Offset { get; set; }
        public BoundingSphere Bounds { get; set; }
        public MaterialReferences Material { get; set; }
    }
}
