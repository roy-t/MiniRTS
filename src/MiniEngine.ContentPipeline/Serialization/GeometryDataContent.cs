using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace MiniEngine.ContentPipeline.Serialization
{
    internal sealed class GeometryDataContent
    {
        public GeometryDataContent(string name, VertexBufferContent vertices, IndexCollection indices, BoundingSphere bounds)
        {
            this.Name = name;
            this.Vertices = vertices;
            this.Indices = indices;
            this.Bounds = bounds;
        }

        public string Name { get; }

        public VertexBufferContent Vertices { get; }

        public IndexCollection Indices { get; }

        public BoundingSphere Bounds { get; }
    }
}
