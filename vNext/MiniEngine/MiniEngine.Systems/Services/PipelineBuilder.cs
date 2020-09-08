using System;
using System.Collections.Generic;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Injection;
using MiniEngine.Systems.Pipeline;

namespace MiniEngine.Systems.Services
{
    [Service]
    public sealed class PipelineBuilder
    {
        public sealed class IntermediatePipelineBuilder
        {
            private readonly List<SystemSpec> SystemSpecs;
            private readonly Dictionary<Type, IComponentContainer> ComponentContainers;
            private readonly Resolve ResolveDelegate;

            internal IntermediatePipelineBuilder(Resolve resolveDelegate)
            {
                this.SystemSpecs = new List<SystemSpec>();
                this.ComponentContainers = new Dictionary<Type, IComponentContainer>();
                this.ResolveDelegate = resolveDelegate;
            }

            public IntermediatePipelineBuilder AddSystem<T>(Func<SystemSpec, SystemSpec> chain)
           where T : ISystem
            {
                var spec = SystemSpec.Construct<T>();
                this.SystemSpecs.Add(chain(spec));
                return this;
            }

            public IntermediatePipelineBuilder AddComponentContainer<T>(IComponentContainer componentContainer)
                 where T : IComponent
            {
                this.ComponentContainers.Add(typeof(T), componentContainer);
                return this;
            }

            public ParallelPipeline Build()
            {
                var ordered = CoffmanGrahamOrderer.Order(this.SystemSpecs);
                var stages = CoffmanGrahamOrderer.DivideIntoStages(ordered);

                var pipelineStages = new List<PipelineStage>();
                for (var i = 0; i < stages.Count; i++)
                {
                    var systemBindings = new List<ISystemBinding>();
                    var stage = stages[i];
                    for (var j = 0; j < stage.Count; j++)
                    {
                        var systemSpec = stage[j];

                        var system = (ISystem)this.ResolveDelegate(systemSpec.SystemType);
                        systemBindings.AddRange(SystemBinder.BindSystem(system, this.ComponentContainers));

                    }

                    pipelineStages.Add(new PipelineStage(systemBindings));
                }

                return new ParallelPipeline(pipelineStages);
            }
        }


        private readonly Resolve ResolveDelegate;
        public PipelineBuilder(Resolve resolveDelegate)
        {
            this.ResolveDelegate = resolveDelegate;
        }

        public IntermediatePipelineBuilder Builder() => new IntermediatePipelineBuilder(this.ResolveDelegate);
    }
}
