using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Rendering.Pipelines
{
    public sealed class ModelPipeline
    {
        private readonly List<IModelPipelineStage> Stages;

        public ModelPipeline(GraphicsDevice device)
        {
            this.Device = device;
            this.Stages = new List<IModelPipelineStage>();
        }

        public GraphicsDevice Device { get; }

        public void Add(IModelPipelineStage stage) => this.Stages.Add(stage);

        public void Execute(PerspectiveCamera camera, ModelBatchList modelBatchList)
        {
            foreach (var stage in this.Stages)
            {
                stage.Execute(camera, modelBatchList.OpaqueBatch);
            }

            foreach (var batch in modelBatchList.TransparentBatches)
            {
                foreach (var stage in this.Stages)
                {
                    stage.Execute(camera, batch);
                }
            }
        }

        public static ModelPipeline Create(GraphicsDevice device) => new ModelPipeline(device);
    }
}