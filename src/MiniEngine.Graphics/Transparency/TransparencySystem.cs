using MiniEngine.Configuration;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Transparency
{
    [System]
    public partial class TransparencySystem : ISystem
    {
        // TODO: make 2 RTs for transparency, let this system render all transparent particles to it
        // and then resolve it and render it to the light buffer. THe targets should do proper linear colours!!!
        // TODO: double check that the other particle system converts to linear colour first.. it doesn't right now?

        public void OnSet()
        {

        }


        [ProcessAll]
        public void Process(TransparentParticleEmitterComponent emitter, TransformComponent transform)
        {

        }
    }
}
