using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class UVAnimation : IComponent
    {
        public UVAnimation(Entity entity, IndexOffset[] indexOffsets)
        {
            this.Entity = entity;
            this.MeshUVOffsets = indexOffsets;
        }

        public Entity Entity { get; }

        public IndexOffset[] MeshUVOffsets { get; }
    }

    public struct IndexOffset
    {
        public string MeshName { get; }
        public int MeshIndex { get; }
        public Vector2 UVOffset { get; set; }

        public IndexOffset(string meshName, int meshIndex)
        {
            this.MeshName = meshName;
            this.MeshIndex = meshIndex;
            this.UVOffset = Vector2.Zero;
        }
    }
}
