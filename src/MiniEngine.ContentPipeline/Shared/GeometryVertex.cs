using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.ContentPipeline.Shared
{
    // TODO: move back to Graphics project?
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GeometryVertex : IVertexType
    {
        internal static VertexDeclarationContent GetContent()
        {
            var declaration = new VertexDeclarationContent();
            foreach (var element in Declaration.GetVertexElements())
            {
                declaration.VertexElements.Add(element);
            }

            declaration.VertexStride = 8 * 4;

            return declaration;
        }

        public static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
           new VertexElement(0 * 4, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
           new VertexElement(3 * 4, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
           new VertexElement(5 * 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        public Vector3 Position;
        public Vector2 Texture;
        public Vector3 Normal;

        public GeometryVertex(Vector3 position, Vector2 texture, Vector3 normal)
        {
            this.Position = position;
            this.Texture = texture;
            this.Normal = normal;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
