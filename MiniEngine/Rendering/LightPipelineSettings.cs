namespace MiniEngine.Rendering
{
    public sealed class LightPipelineSettings
    {
        public LightPipelineSettings()
        {
            this.EnableAmbientLights = true;
            this.EnableDirectionalLights = true;
            this.EnablePointLights = true;

            this.EnableShadowCastingLights = true;
            this.ShadowCastingLightsResolution = 1024;

            this.EnableSunLights = true;
            this.SunLightsResolution = 2048;
        }


        public bool EnableAmbientLights { get; set; }
        public bool EnableDirectionalLights { get; set; }
        public bool EnablePointLights { get; set; }

        public bool EnableShadowCastingLights { get; set; }
        public int ShadowCastingLightsResolution { get; set; }

        public bool EnableSunLights { get; set; }
        public int SunLightsResolution { get; set; }
    }
}
