using System;
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
        private readonly IEnumerable<IComponentContainer> ComponentContainers;

        public PipelineBuilder(Resolve resolveDelegate, IEnumerable<IComponentContainer> componentContainers)
        {
            this.ResolveDelegate = resolveDelegate;
            this.ComponentContainers = componentContainers;
        }

        public PipelineSpecifier Builder() => new PipelineSpecifier(this.ResolveDelegate, this.ComponentContainers);

        public class PipelineSpecifier
        {
            private readonly Dictionary<Type, IComponentContainer> ComponentContainers;
            private readonly Resolve ResolveDelegate;
            private readonly List<SystemSpec> SystemSpecs;

            public PipelineSpecifier(Resolve resolveDelegate, IEnumerable<IComponentContainer> componentContainers)
            {
                this.SystemSpecs = new List<SystemSpec>();

                this.ComponentContainers = new Dictionary<Type, IComponentContainer>();
                foreach (var componentContainer in componentContainers)
                {
                    this.ComponentContainers.Add(componentContainer.ComponentType, componentContainer);
                }

                this.ResolveDelegate = resolveDelegate;
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

            public SystemSpecifier Produces(string resource, string state)
            {
                this.Spec.Produces(resource, state);
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
