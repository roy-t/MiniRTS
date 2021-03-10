using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Particles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new(
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 1)
        );

        public Vector2 UV;
        public VertexDeclaration VertexDeclaration => Declaration;

        public Particle(Vector2 uv)
        {
            this.UV = uv;
        }
    }
}
