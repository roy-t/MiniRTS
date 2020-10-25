using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Skybox
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SkyboxVertex : IVertexType
    {
        private static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
        );

        public Vector3 Position;

        public SkyboxVertex(float x, float y, float z)
            : this(new Vector3(x, y, z)) { }

        public SkyboxVertex(Vector3 position)
        {
            this.Position = position;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
