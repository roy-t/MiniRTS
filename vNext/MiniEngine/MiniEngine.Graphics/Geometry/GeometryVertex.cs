using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Geometry
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct GeometryVertex : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
           new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
           new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
        );

        public Vector3 Position;
        public Vector3 Normal;

        public GeometryVertex(Vector3 position, Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
