namespace MiniEngine.Rendering
{
    public sealed class ModelPipelineSettings
    {
        public ModelPipelineSettings()
        {
            this.FxaaFactor = 2;
        }

        public int FxaaFactor { get; set; }
    }
}
