using Microsoft.Xna.Framework;

namespace MiniEngine.Gui.Tools.Vectors
{
    public static class VectorUtils
    {
        public static string ToShortString(Vector3 vector) => $"{{{vector.X:F2} {vector.Y:F2} {vector.Z:F2}}}";
    }
}
