using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Physics
{
    [System]
    public partial class ForcesSystem : ISystem
    {
        private readonly FrameService FrameService;

        public ForcesSystem(FrameService frameService)
        {
            this.FrameService = frameService;
        }

        public void OnSet() { }

        [ProcessAll]
        public void UpdateForces(ForcesComponent forces, TransformComponent transform)
        {
            var elapsed = this.FrameService.Elapsed;

            forces.LastVelocity = forces.Velocity;
            forces.LastPosition = forces.Position;

            forces.Position = transform.Position;
            forces.Velocity = (forces.Position - forces.LastPosition) / elapsed;
            forces.Acceleration = (forces.Velocity - forces.LastVelocity) / elapsed;
        }
    }
}
