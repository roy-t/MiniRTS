using System.Collections.Generic;
using MiniEngine.Configuration;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Pipeline;

namespace MiniEngine.Systems.Services
{
    [Service]
    public sealed class PipelineBuilder
    {
        private readonly Resolve ResolveDelegate;
        private readonly ContainerStore ContainerStore;

        public PipelineBuilder(Resolve resolveDelegate, ContainerStore containerStore)
        {
            this.ResolveDelegate = resolveDelegate;
            this.ContainerStore = containerStore;
        }

        public PipelineSpecifier Builder() => new PipelineSpecifier(this.ResolveDelegate, this.ContainerStore);

        public class PipelineSpecifier
        {
            private readonly Resolve ResolveDelegate;
            private readonly ContainerStore ContainerStore;
            private readonly List<SystemSpec> SystemSpecs;

            public PipelineSpecifier(Resolve resolveDelegate, ContainerStore containerStore)
            {
                this.SystemSpecs = new List<SystemSpec>();
                this.ResolveDelegate = resolveDelegate;
                this.ContainerStore = containerStore;
            }

            public SystemSpecifier System<T>()
                where T : ISystem
            {
                var spec = SystemSpec.Construct<T>();
                this.SystemSpecs.Add(spec);

                return new SystemSpecifier(this, spec);
            }

            public ParallelPipeline Build()
            {
                var stages = CoffmanGrahamOrderer.DivideIntoStages(this.SystemSpecs);

                var pipelineStages = new List<PipelineStage>();
                for (var i = 0; i < stages.Count; i++)
                {
                    var systemBindings = new List<ISystemBinding>();
                    var stage = stages[i];
                    for (var j = 0; j < stage.Count; j++)
                    {
                        var systemSpec = stage[j];
                        var system = (ISystem)this.ResolveDelegate(systemSpec.SystemType);
                        systemBindings.Add(system.Bind(this.ContainerStore));
                    }

                    pipelineStages.Add(new PipelineStage(systemBindings));
                }

                return new ParallelPipeline(pipelineStages);
            }
        }

        public class SystemSpecifier
        {
            private readonly PipelineSpecifier Parent;
            private readonly SystemSpec Spec;

            public SystemSpecifier(PipelineSpecifier parent, SystemSpec spec)
            {
                this.Parent = parent;
                this.Spec = spec;
            }

            public SystemSpecifier Requires(string resource, string state)
            {
                this.Spec.Requires(resource, state);
                return this;
            }

            public SystemSpecifier RequiresAll(string resource)
            {
                this.Spec.RequiresAll(resource);
                return this;
            }

            public SystemSpecifier Produces(string resource, string state)
            {
                this.Spec.Produces(resource, state);
                return this;
            }

            public SystemSpecifier Produces(string resource)
            {
                this.Spec.Produces(resource);
                return this;
            }

            public SystemSpecifier Parallel()
            {
                this.Spec.Parallel();
                return this;
            }

            public SystemSpecifier InSequence()
            {
                this.Spec.InSequence();
                return this;
            }

            public PipelineSpecifier Build()
                => this.Parent;
        }
    }
}
