using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Basics.Systems
{
    public sealed class OffsetSystem : IUpdatableSystem
    {
        private readonly IComponentContainer<Offset> Offsets;
        private readonly IComponentContainer<Pose> Poses;

        public OffsetSystem(IComponentContainer<Offset> offsets, IComponentContainer<Pose> poses)
        {
            this.Offsets = offsets;
            this.Poses = poses;
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            for (var i = 0; i < this.Offsets.Count; i++)
            {
                var offset = this.Offsets[i];
                var myPose = this.Poses.Get(offset.Entity);
                var targetPose = this.Poses.Get(offset.Target);

                myPose.PlaceAtOffset(offset, targetPose);
            }
        }
    }
}
