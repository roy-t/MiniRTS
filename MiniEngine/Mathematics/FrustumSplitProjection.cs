using Microsoft.Xna.Framework;

namespace MiniEngine.Mathematics
{
    public class FrustumSplitProjection
    {
        public FrustumSplitProjection(float distance, Matrix projection)
        {
            this.Distance = distance;
            this.Projection = projection;
        }

        public float Distance { get; }
        public Matrix Projection { get; }
    }
}
