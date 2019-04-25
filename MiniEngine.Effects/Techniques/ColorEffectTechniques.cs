namespace MiniEngine.Effects.Techniques
{
    public enum ColorEffectTechniques
    {
        /// <summary>
        /// Draw colored primitives tinted with VisibileTint
        /// </summary>
        Color,

        /// <summary>
        /// Draw colored primitives, perform a depth test based on the geometry's position for each pixel covered by the primitive.
        /// Visibile pixels are tinted with VisibileTint, occluded pixels are tinted with ClippedTint.
        /// </summary>
        ColorGeometryDepthTest,

        /// <summary>
        /// Draw colored primitives, perform a depth test based on WorldPosition for each pixel covered by the primitive.
        /// Visibile pixels are tinted with VisibileTint, occluded pixels are tinted with ClippedTint.
        /// </summary>
        ColorPointDepthTest
    }
}
