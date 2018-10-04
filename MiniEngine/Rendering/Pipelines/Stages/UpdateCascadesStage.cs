﻿using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class UpdateCascadesStage : IPipelineStage
    {
        private readonly SunlightSystem SunlightSystem;

        public UpdateCascadesStage(SunlightSystem sunlightSystem)
        {
            this.SunlightSystem = sunlightSystem;
        }

        public void Execute(PerspectiveCamera camera, Seconds _)
        {
            this.SunlightSystem.Update(camera);
        }
    }
}
