namespace MiniEngine.Effects.Techniques
{
    public enum RenderEffectTechniques
    {
        /// <summary>
        /// Renders the depth
        /// </summary>
        ShadowMap,

        /// <summary>
        /// Renders depth, taking into account animations/skinning of models
        /// </summary>
        ShadowMapSkinned,

        /// <summary>
        /// Renders diffuse textures
        /// </summary>
        Textured,

        /// <summary>
        /// Renders all material properties for deferred rendering
        /// </summary>
        Deferred,

        /// <summary>
        /// Renders all material properties for deferred rendering, 
        /// taking into account animations/skinning of models
        /// </summary>
        DeferredSkinned
    }
}