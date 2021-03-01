using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Particles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Particle : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
            new VertexElement(12, VertexElementFormat.Color, VertexElementUsage.Color, 1),
            new VertexElement(16, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(20, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3),
            new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 4)
        );

        public Vector3 Position;
        public Color Color;
        public float Scale;
        public float Metalicness;
        public float Roughness;
        public float Energy; // Currently unused in shader

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
