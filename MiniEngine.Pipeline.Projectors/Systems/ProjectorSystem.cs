using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Effects.DeviceStates;
using System.Collections.Generic;
using MiniEngine.Effects;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class ProjectorSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FullScreenTriangle FullScreenTriangle;

        private readonly EntityLinker EntityLinker;
        private readonly ProjectorEffect Effect;
        private readonly List<Projector> Projectors;        

        public ProjectorSystem(GraphicsDevice device, EntityLinker entityLinker, ProjectorEffect effect)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
            this.Effect = effect;

            this.FullScreenTriangle = new FullScreenTriangle();

            this.Projectors = new List<Projector>();
        }

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
                    this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;

                    this.Effect.Apply();

                    this.FullScreenTriangle.Render(this.Device);
                }
            }
        }
    }
}
