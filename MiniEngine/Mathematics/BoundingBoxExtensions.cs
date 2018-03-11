using Microsoft.Xna.Framework;

namespace MiniEngine.Mathematics
{
    public static class BoundingBoxExtensions
    {
        // From: http://dev.theomader.com/cascaded-shadow-mapping-1/

        public static BoundingBox Transform(this BoundingBox boundingBox, Matrix m)
        {
            var xa = m.Right * boundingBox.Min.X;
            var xb = m.Right * boundingBox.Max.X;

            var ya = m.Up * boundingBox.Min.Y;
            var yb = m.Up * boundingBox.Max.Y;

            var za = m.Backward * boundingBox.Min.Z;
            var zb = m.Backward * boundingBox.Max.Z;

            return new BoundingBox(
                Vector3.Min(xa, xb) + Vector3.Min(ya, yb) + Vector3.Min(za, zb) + m.Translation,
                Vector3.Max(xa, xb) + Vector3.Max(ya, yb) + Vector3.Max(za, zb) + m.Translation
            );
        }
    }
}
