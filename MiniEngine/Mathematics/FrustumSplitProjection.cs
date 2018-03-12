using Microsoft.Xna.Framework;
using MiniEngine.Rendering;

namespace MiniEngine.Mathematics
{
    public class FrustumSplitProjection : IViewPoint
    {        
        public FrustumSplitProjection(float distance, Matrix projection, Matrix view)
        {
            this.Distance = distance;
            this.Projection = projection;
            this.View = view;
        }

        public float Distance { get; }
        public Matrix View { get; }
        public Matrix Projection { get; }
    }
}
