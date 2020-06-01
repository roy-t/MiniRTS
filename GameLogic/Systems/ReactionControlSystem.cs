using Microsoft.Xna.Framework;
using MiniEngine.GameLogic.Vehicles.Fighter;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Systems
{
    public sealed class ReactionControlSystem : IUpdatableSystem
    {
        private readonly IComponentContainer<ReactionControl> ReactionControllers;
        private readonly IComponentContainer<AdditiveEmitter> Emitters;
        private readonly IComponentContainer<Pose> Poses;

        // TODO: how to enable any kind of emitter?

        public ReactionControlSystem(
            IComponentContainer<ReactionControl> reactionControllers,
            IComponentContainer<AdditiveEmitter> emitters,
            IComponentContainer<Pose> poses)
        {
            this.ReactionControllers = reactionControllers;
            this.Emitters = emitters;
            this.Poses = poses;
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            for (var i = 0; i < this.ReactionControllers.Count; i++)
            {
                var rcs = this.ReactionControllers[i];
                var newPosition = this.Poses.Get(rcs.Entity).Position;

                UpdateAccelerationAndVelocity(rcs, newPosition, elapsed);

                for (var e = 0; e < rcs.Emitters.Length; e++)
                {
                    var emitter = this.Emitters.Get(rcs.Emitters[e]);
                    var dot = Vector3.Dot(emitter.Direction, -Vector3.Normalize(rcs.Acceleration));
                    if (dot > rcs.EmitterReactionRange)
                    {
                        emitter.StartVelocity = rcs.Velocity;
                        emitter.Enabled = true;
                    }
                    else
                    {
                        emitter.Enabled = false;
                    }
                }
            }
        }

        public static void UpdateAccelerationAndVelocity(ReactionControl rcs, Vector3 newPosition, Seconds elapsed)
        {
            var newVelocity = (newPosition - rcs.LastPosition) / elapsed;
            rcs.Acceleration = (newVelocity - rcs.Velocity) / elapsed;

            rcs.Velocity = newVelocity;
            rcs.LastPosition = newPosition;
        }
    }
}
