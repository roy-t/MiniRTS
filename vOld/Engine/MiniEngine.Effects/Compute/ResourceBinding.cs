using SharpDX.Direct3D11;

namespace MiniEngine.Effects.Compute
{
    sealed class ResourceBinding
    {
        public ResourceBinding(int register, string name, ShaderResourceType resourceType)
        {
            this.Register = register;
            this.Name = name;
            this.ResourceType = resourceType;
        }

        public int Register { get; }
        public string Name { get; }
        public ShaderResourceType ResourceType { get; }

        public SharpDX.Direct3D11.Buffer Buffer { get; set; }
        public ResourceView View { get; set; }

        internal void Dispose()
        {
            this.Buffer?.Dispose();
            this.View?.Dispose();
        }
    }
}
