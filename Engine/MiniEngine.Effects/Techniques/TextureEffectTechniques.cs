namespace MiniEngine.Effects.Techniques
{
    public enum TextureEffectTechniques
    {
        /// <summary>
        /// Draw textured primitives tinted with VisibileTint
        /// </summary>
        Texture,

        /// <summary>
        /// Draw textured primitives, perform a depth test based on the geometry's position for each pixel covered by the primitive.
        /// Visibile pixels are tinted with VisibileTint, occluded pixels are tinted with ClippedTint.
        /// </summary>
        TextureGeometryDepthTest,

        /// <summary>
        /// Draw textured primitives, perform a depth test based on WorldPosition for each pixel covered by the primitive.
        /// Visibile pixels are tinted with VisibileTint, occluded pixels are tinted with ClippedTint.
        /// </summary>
        TexturePointDepthTest
    }
}
