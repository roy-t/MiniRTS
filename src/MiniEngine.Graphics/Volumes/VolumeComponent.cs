using MiniEngine.ContentPipeline.Shared;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Volumes
{
    public sealed class VolumeComponent : AComponent
    {
        public VolumeComponent(Entity entity, Material material)
            : base(entity)
        {
            this.Material = material;
        }

        public Material Material { get; set; }
    }
}
