using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniEngine.Systems.Pipeline
{
    public sealed class SystemSpec
    {
        private readonly List<ResourceState> RequiresList;
        private readonly List<ResourceState> ProducesList;

        public const string MatchAllSubResources = "*";

        private SystemSpec(Type systemType)
        {
            this.SystemType = systemType;
            this.RequiresList = new List<ResourceState>();
            this.ProducesList = new List<ResourceState>();
            this.AllowParallelism = false;
        }

        public static SystemSpec Construct<T>()
            where T : ISystem
            => new SystemSpec(typeof(T));

        public Type SystemType { get; }

        public bool AllowParallelism { get; private set; }

        internal IReadOnlyList<ResourceState> RequiredResources => this.RequiresList;
        internal IReadOnlyList<ResourceState> ProducedResources => this.ProducesList;

        public SystemSpec Requires(string resource, string subResource)
        {
            this.RequiresList.Add(new ResourceState(resource, subResource));
            return this;
        }

        public SystemSpec RequiresAll(string resource)
            => this.Requires(resource, MatchAllSubResources);

        public SystemSpec Produces(string resource, string? state)
        {
            this.ProducesList.Add(new ResourceState(resource, state));
            return this;
        }

        public SystemSpec Produces(string resource)
            => this.Produces(resource, null);

        public SystemSpec Parallel()
        {
            this.AllowParallelism = true;
            return this;
        }

        public SystemSpec InSequence()
        {
            this.AllowParallelism = false;
            return this;
        }

        public override string ToString()
            => $"{this.SystemType.Name}: " +
            $"allow parallelism: {this.AllowParallelism}, " +
            $"requires: [{string.Join(", ", this.RequiredResources)}], " +
            $"produces: [{string.Join(", ", this.ProducedResources)}]";

        internal void ExpandRequiredResource(Dictionary<string, List<ResourceState>> producedResources)
        {
            for (var i = this.RequiresList.Count - 1; i >= 0; i--)
            {
                var resource = this.RequiresList[i];
                if (resource.SubResource == MatchAllSubResources)
                {
                    this.RequiresList.RemoveAt(i);
                }

                var produces = producedResources[resource.Resource].Select(s => new ResourceState(s.Resource, s.SubResource));
                this.RequiresList.AddRange(produces);
            }
        }
    }
}
