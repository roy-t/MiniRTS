using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiniEngine.Configuration;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Pipeline;
using Serilog;

namespace MiniEngine.Systems.Services
{
    [Service]
    public sealed class PipelineBuilder
    {

        private readonly ILogger Logger;

        private readonly Resolve ResolveDelegate;
        private readonly ContainerStore ContainerStore;

        public PipelineBuilder(Resolve resolveDelegate, ContainerStore containerStore, ILogger logger)
        {
            this.ResolveDelegate = resolveDelegate;
            this.ContainerStore = containerStore;
            this.Logger = logger;
        }

        public PipelineSpecifier Builder() => new PipelineSpecifier(this.ResolveDelegate, this.ContainerStore, this.Logger);

        public class PipelineSpecifier
        {
            private readonly Resolve ResolveDelegate;
            private readonly ContainerStore ContainerStore;
            private readonly ILogger Logger;
            private readonly List<SystemSpec> SystemSpecs;

            public PipelineSpecifier(Resolve resolveDelegate, ContainerStore containerStore, ILogger logger)
            {
                this.SystemSpecs = new List<SystemSpec>();
                this.ResolveDelegate = resolveDelegate;
                this.ContainerStore = containerStore;
                this.Logger = logger;
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

                var text = new StringBuilder();
                text.AppendLine("Parallel Pipeline:");
                PrintPipeline(text, pipelineStages);

                this.Logger.Information(text.ToString());

                return new ParallelPipeline(pipelineStages);
            }

            private static void PrintPipeline(StringBuilder text, IReadOnlyList<PipelineStage> stages)
            {
                for (var i = 0; i < stages.Count; i++)
                {
                    var stage = stages[i];
                    text.Append($"[{i}]: ");
                    text.AppendJoin(", ", stage.Systems.Select(system => system.GetType().Name));
                    text.AppendLine();
                }
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

            public SystemSpecifier Requires(Enum resource, Enum state)
                => this.Requires(resource.ToString(), state.ToString());

            public SystemSpecifier RequiresAll(string resource)
            {
                this.Spec.RequiresAll(resource);
                return this;
            }

            public SystemSpecifier RequiresAll(Enum resource)
                => this.RequiresAll(resource.ToString());

            public SystemSpecifier Produces(string resource, string state)
            {
                this.Spec.Produces(resource, state);
                return this;
            }

            public SystemSpecifier Produces(Enum resource, Enum state)
                => this.Produces(resource.ToString(), state.ToString());

            public SystemSpecifier Produces(string resource)
            {
                this.Spec.Produces(resource);
                return this;
            }

            public SystemSpecifier Produces(Enum resource)
                => this.Produces(resource.ToString());

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
