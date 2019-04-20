using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class ProjectorSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly BoundsDrawer3D FrustumDrawer;

        private readonly EntityLinker EntityLinker;
        private readonly ProjectorEffect Effect;
        private readonly ColorEffect ColorEffect;
        private readonly List<Projector> Projectors;

        public ProjectorSystem(GraphicsDevice device, EntityLinker entityLinker, ProjectorEffect effect, ColorEffect colorEffect)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
            this.Effect = effect;
            this.ColorEffect = colorEffect;

            this.FrustumDrawer = new BoundsDrawer3D(device);
            this.Technique = ProjectorEffectTechniques.Projector;

            this.Projectors = new List<Projector>();
        }

        public ProjectorEffectTechniques Technique { get; set; }

        public void RenderProjectors(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            this.Projectors.Clear();
            this.EntityLinker.GetComponents(this.Projectors);

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
                    this.Effect.ProjectorViewProjection = projector.ViewPoint.ViewProjection;
                    this.Effect.MaxDistance = projector.MaxDistance;
                    this.Effect.ProjectorPosition = projector.ViewPoint.Position;
                    this.Effect.ProjectorForward = projector.ViewPoint.Forward;
                    this.Effect.Tint = projector.Tint;

                    // Camera properties
                    this.Effect.World = Matrix.Identity;
                    this.Effect.View = perspectiveCamera.View;
                    this.Effect.Projection = perspectiveCamera.Projection;
                    this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;
                                       
                    this.Effect.Apply(this.Technique);                    

                    this.FrustumDrawer.Render(projector.ViewPoint.Frustum);                    
                    this.DrawBoundingBox(projector.ViewPoint.Frustum, perspectiveCamera);
                }
            }
        }

        // TODO: move the outline system and make sure it can draw projectors as well so that we don't need this code!
        private void DrawBoundingBox(BoundingFrustum frustum, PerspectiveCamera camera)
        {
            this.ColorEffect.World = Matrix.Identity;
            this.ColorEffect.View = camera.View;
            this.ColorEffect.Projection = camera.Projection;
            this.ColorEffect.Color = Color.Purple;

            this.ColorEffect.Apply();
            
            this.FrustumDrawer.RenderOutline(frustum);          
        }        
    }
}
