using MiniEngine.Effects.Techniques;

namespace MiniEngine.Rendering
{
    public sealed class RenderPipelineSettings
    {
        public RenderPipelineSettings()
        {
            this.LightSettings = new LightPipelineSettings();
            this.ModelSettings = new ModelPipelineSettings();

            this.EnableShadows = true;
            this.EnableModels = true;
            this.EnableParticles = true;
            this.Enable3DOutlines = true;
            this.Enable2DOutlines = true;
            this.EnableProjectors = true;
        }

        public LightPipelineSettings LightSettings { get; private set; }
        public ModelPipelineSettings ModelSettings { get; private set; }

        public bool EnableShadows { get; set; }
        public bool EnableModels { get; set; }
        public bool EnableParticles { get; set; }
        public bool Enable3DOutlines { get; set; }
        public bool Enable2DOutlines { get; set; }

        public bool EnableProjectors { get; set; }
        public ProjectorEffectTechniques ProjectorTechnique { get; set; }
    }
}
