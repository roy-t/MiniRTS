using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Pipeline.Models.Stages;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class AntiAliasingStageExtensions
    {
        public static ModelPipeline AntiAlias(
            this ModelPipeline pipeline,
            FxaaEffect effect,
            RenderTarget2D sourceTarget,
            RenderTarget2D destinationTarget,
            float strength)
        {
            var stage = new AntiAliasStage(pipeline.Device, effect, sourceTarget, destinationTarget, strength);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}