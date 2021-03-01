using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Particles
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ParticleInstancingVertex : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 1),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Color, 1),
            new VertexElement(24, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 1),
            new VertexElement(28, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 2),
            new VertexElement(32, VertexElementFormat.Single, VertexElementUsage.TextureCoordinate, 3)
        );

        public Vector3 Position;
        public Vector3 Color; // TODO: encode color as uint and decode in shader?
        public float Scale;
        public float Metalicness;
        public float Roughness;

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
