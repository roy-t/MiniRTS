using System;
using System.Collections.Generic;
using MiniEngine.Systems.Next;

namespace MiniEngine.Pipeline.Next
{
    public sealed class SystemSpec
    {
        private readonly List<ResourceState> RequiresList;
        private readonly List<ResourceState> ProducesList;

        private SystemSpec(Type systemType)
        {
            this.SystemType = systemType;
            this.RequiresList = new List<ResourceState>();
            this.ProducesList = new List<ResourceState>();
        }

        public static SystemSpec Construct<T>()
            where T : ISystem
            => new SystemSpec(typeof(T));

        public Type SystemType { get; }

        public bool AllowParallelism { get; set; }

        internal IReadOnlyList<ResourceState> RequiredResources => this.RequiresList;
        internal IReadOnlyList<ResourceState> ProducedResources => this.ProducesList;

        public SystemSpec Requires(string resource, string state)
        {
            this.RequiresList.Add(new ResourceState(resource, state));
            return this;
        }

        public SystemSpec Produces(string resource, string state)
        {
            this.ProducesList.Add(new ResourceState(resource, state));
            return this;
        }

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
            $"requires: [{string.Join(", ", this.RequiredResources)}], " +
            $"produces: [{string.Join(", ", this.ProducedResources)}]";
    }
}
