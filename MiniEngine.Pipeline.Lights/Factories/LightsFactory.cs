namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class LightsFactory
    {
        public LightsFactory(AmbientLightFactory ambientLightFactory, DirectionalLightFactory directionalLightFactory,
            PointLightFactory pointLightFactory, ShadowCastingLightFactory shadowCastingLightFactory,
            SunlightFactory sunlightFactory)
        {
            this.AmbientLightFactory = ambientLightFactory;
            this.DirectionalLightFactory = directionalLightFactory;
            this.PointLightFactory = pointLightFactory;
            this.ShadowCastingLightFactory = shadowCastingLightFactory;
            this.SunlightFactory = sunlightFactory;
        }

        public AmbientLightFactory AmbientLightFactory { get; }
        public DirectionalLightFactory DirectionalLightFactory { get; }
        public PointLightFactory PointLightFactory { get; }
        public ShadowCastingLightFactory ShadowCastingLightFactory { get; }
        public SunlightFactory SunlightFactory { get; }
    }
}
