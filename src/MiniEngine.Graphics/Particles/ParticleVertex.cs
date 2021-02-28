using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Particles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ParticleVertex : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
           new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
        );

        public Vector3 Position;

        public ParticleVertex(Vector3 position)
        {
            this.Position = position;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
