using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles
{
    public static class ParticleMath
    {
        // MonoGame's own Matrix.CreateBillboard incorrectly flips the image over the Y axis 
        // and aligns the billboard with the center of the view, not perfectly with the screen.
        public static Matrix CreateBillboard(Vector3 position, Matrix view)
        {
            var result = Matrix.Identity;
            result.Translation = position;
            result.M11 = view.M11;
            result.M12 = view.M21;
            result.M13 = view.M31;

            result.M21 = view.M12;
            result.M22 = view.M22;
            result.M23 = view.M32;

            result.M31 = view.M13;
            result.M32 = view.M23;
            result.M33 = view.M33;

            return result;
        }
    }
}
