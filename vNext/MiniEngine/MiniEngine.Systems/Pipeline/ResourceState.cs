using System;

namespace MiniEngine.Systems.Pipeline
{
    public sealed class ResourceState
    {
        public ResourceState(string resource, string state)
        {
            this.Resource = resource;
            this.State = state;
        }

        public string Resource { get; }
        public string State { get; }

        public override bool Equals(object? obj) => obj is ResourceState state && this.Resource == state.Resource && this.State == state.State;
        public override int GetHashCode() => HashCode.Combine(this.Resource, this.State);

        public override string ToString() => $"({this.Resource} - {this.State})";
    }
}
