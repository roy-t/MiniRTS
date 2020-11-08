using System;

namespace MiniEngine.Systems.Pipeline
{
    public sealed class ResourceState
    {
        public ResourceState(string resource, string? subResource = null)
        {
            this.Resource = resource;
            this.SubResource = subResource;
        }

        public string Resource { get; }

        public string? SubResource { get; }

        public override bool Equals(object? obj) => obj is ResourceState state && this.Resource == state.Resource && this.SubResource == state.SubResource;

        public override int GetHashCode() => HashCode.Combine(this.Resource, this.SubResource);

        public override string ToString() => this.SubResource != null ? $"({this.Resource} - {this.SubResource})" : $"({this.Resource})";
    }
}
