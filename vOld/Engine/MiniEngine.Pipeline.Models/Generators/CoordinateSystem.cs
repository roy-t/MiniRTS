using Microsoft.Xna.Framework;

namespace MiniEngine.Pipeline.Models.Generators
{
    readonly struct CoordinateSystem
    {
        public CoordinateSystem(Vector3 unitX, Vector3 unitY, Vector3 unitZ)
        {
            this.UnitX = unitX;
            this.UnitY = unitY;
            this.UnitZ = unitZ;
        }

        public Vector3 UnitX { get; }
        public Vector3 UnitY { get; }
        public Vector3 UnitZ { get; }
    }
}

