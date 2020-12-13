namespace MiniEngine.Effects.Techniques
{
    public enum ProjectorEffectTechniques
    {
        /// <summary>
        /// Project the selected texture onto the geometry
        /// </summary>
        Projector,

        /// <summary>
        /// Project the selected texture onto the geometry, and color the areas for which
        /// the pixel shader also ran, but on which no texture was projected.
        /// </summary>
        ProjectorOverdraw
    }
}
