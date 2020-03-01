namespace MiniEngine.Effects.Techniques
{
    public enum RenderEffectTechniques
    {
        /// <summary>
        /// Renders the depth
        /// </summary>
        ShadowMap,

        /// <summary>
        /// Renders diffuse textures
        /// </summary>
        Textured,

        /// <summary>
        /// Renders all material properties for deferred rendering
        /// </summary>
        Deferred,

        DeferredSkinned
    }
}