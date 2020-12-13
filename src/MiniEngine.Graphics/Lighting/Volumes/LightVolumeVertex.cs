using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Lighting.Volumes
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LightVolumeVertex : IVertexType
    {
        public static readonly VertexDeclaration Declaration = new VertexDeclaration
        (
           new VertexElement(0 * 4, VertexElementFormat.Vector3, VertexElementUsage.Position, 0)
        );

        public Vector3 Position;

        public LightVolumeVertex(Vector3 position)
        {
            this.Position = position;
        }

        public VertexDeclaration VertexDeclaration => Declaration;
    }
}
