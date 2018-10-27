using Microsoft.Xna.Framework;

namespace MiniEngine.Pipeline.Lights.Utilities
{
    public static class MatrixExtensions
    {
        private static readonly Matrix TexScaleTransform = Matrix.CreateScale(0.5f, -0.5f, 1.0f)
                                                           * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);

        /// <summary>
        /// Transforms the input matrix so that transformations to [-1, 1] will now fall in [0, 1]
        /// </summary>        
        public static Matrix TextureScaleTransform(this Matrix matrix) => matrix * TexScaleTransform;        
    }
}
