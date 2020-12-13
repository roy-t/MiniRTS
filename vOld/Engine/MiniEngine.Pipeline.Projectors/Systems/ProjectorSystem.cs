using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class ProjectorSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly BoundsDrawer3D FrustumDrawer;

        private readonly IComponentContainer<Projector> Projectors;
        private readonly ProjectorEffect Effect;

        public ProjectorSystem(GraphicsDevice device, IComponentContainer<Projector> projectors, EffectFactory effectFactory)
        {
            this.Device = device;            
            this.Projectors = projectors;
            this.Effect = effectFactory.Construct<ProjectorEffect>();

            this.FrustumDrawer = new BoundsDrawer3D(device);
            this.Technique = ProjectorEffectTechniques.Projector;
        }

        public ProjectorEffectTechniques Technique { get; set; }

        public void RenderProjectors(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            this.Device.PostProcessState();

            for (var i = 0; i < this.Projectors.Count; i++)
            {
                var projector = this.Projectors[i];
                if (projector.ViewPoint.Frustum.Intersects(perspectiveCamera.Frustum))
                {
                    //G-Buffer input
                    this.Effect.DepthMap = gBuffer.DepthTarget;

                    // Projector properties
                    this.Effect.ProjectorMap = projector.Texture;
                    this.Effect.Mask = projector.Mask;
                    this.Effect.ProjectorViewProjection = projector.ViewPoint.ViewProjection;
                    this.Effect.MaxDistance = projector.MaxDistance;
                    this.Effect.ProjectorPosition = projector.ViewPoint.Position;
                    this.Effect.ProjectorForward = projector.ViewPoint.Forward;
                    this.Effect.Tint = projector.Tint;

                    // Camera properties
                    this.Effect.World = Matrix.Identity;
                    this.Effect.View = perspectiveCamera.View;
                    this.Effect.Projection = ProjectionMath.ComputeProjectionMatrixThatFitsFrustum(perspectiveCamera, projector.ViewPoint.Frustum);
                    this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;
                                       
                    this.Effect.Apply(this.Technique);                    

                    this.FrustumDrawer.Render(projector.ViewPoint.Frustum);                    
                }
            }
        }      
    }
}
