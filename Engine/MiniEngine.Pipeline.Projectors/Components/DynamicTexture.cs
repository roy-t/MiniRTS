using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Projectors.Components
{
    public sealed class DynamicTexture : IComponent
    {
        public DynamicTexture(RenderPipeline pipeline, PerspectiveCamera viewPoint, GBuffer gBuffer, Pass pass, string label)
        {
            this.Pipeline = pipeline;            
            this.ViewPoint = viewPoint;
            this.GBuffer = gBuffer;
            this.Pass = pass;
            this.Label = label;
            this.Input = new RenderPipelineInput();            
        }

        public RenderPipeline Pipeline { get; }
        public RenderPipelineInput Input { get; }        

        [Editor(nameof(ViewPoint))]
        public PerspectiveCamera ViewPoint { get; set; }

        public GBuffer GBuffer { get; }
        public Pass Pass { get; }
        
        [Editor(nameof(Label))]
        public string Label { get; }

        [Boundary(BoundaryType.Frustum)]
        public BoundingFrustum Bounds => this.ViewPoint.Frustum;

        [Icon(IconType.Camera)]
        public Vector3 Position => this.ViewPoint.Position;

        [Icon(IconType.LookAt)]
        public Vector3 LookAt => this.ViewPoint.LookAt;

        [Editor(nameof(DiffuseTarget))]
        public Texture2D DiffuseTarget => this.GBuffer.DiffuseTarget;

        [Editor(nameof(NormalTarget))]
        public Texture2D NormalTarget => this.GBuffer.NormalTarget;

        [Editor(nameof(DepthTarget))]
        public Texture2D DepthTarget => this.GBuffer.DepthTarget;

        [Editor(nameof(ParticleTarget))]
        public Texture2D ParticleTarget => this.GBuffer.ParticleTarget;

        [Editor(nameof(LightTarget))]
        public Texture2D LightTarget => this.GBuffer.LightTarget;

        [Editor(nameof(CombineTarget))]
        public Texture2D CombineTarget => this.GBuffer.CombineTarget;

        [Editor(nameof(BlurTarget))]
        public Texture2D BlurTarget => this.GBuffer.BlurTarget;

        [Editor(nameof(FinalTarget))]
        public Texture2D FinalTarget => this.GBuffer.FinalTarget;
    }
}
