using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Pipeline
{
    public sealed class RenderPipelineInput : IPipelineInput
    {
        public void Update(PerspectiveCamera camera, Seconds elapsed, GBuffer gBuffer, Pass pass, TextureCube skybox)
        {
            this.Camera = camera;
            this.Elapsed = elapsed;
            this.GBuffer = gBuffer;
            this.Pass = pass;
            this.Skybox = skybox;
        }

        public PerspectiveCamera Camera { get; private set; }
        public Seconds Elapsed { get; private set; }
        public GBuffer GBuffer { get; private set; }
        public Pass Pass { get; private set; }
        public TextureCube Skybox { get; private set; }
    }
}
