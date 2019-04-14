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
        private readonly WrappableQuad Quad;

        private readonly EntityLinker EntityLinker;
        private readonly ProjectorEffect Effect;
        private readonly List<Projector> Projectors;

        private readonly Vector3[] FrustumCorners;        

        public ProjectorSystem(GraphicsDevice device, EntityLinker entityLinker, ProjectorEffect effect)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
            this.Effect = effect;

            this.Quad = new WrappableQuad(device);
            this.Technique = ProjectorEffectTechniques.Projector;

            this.Projectors = new List<Projector>();

            this.FrustumCorners = new Vector3[8];
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
                    this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;


                    this.Quad.WrapOnScreen(projector.ViewPoint.Frustum, perspectiveCamera.ViewProjection);

                    this.Effect.Apply(this.Technique);
                    this.Quad.Render();                    
                }
            }
        }

        private static readonly Matrix TexScaleTransform = Matrix.CreateScale(0.5f, -0.5f, 1.0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);
        private static Vector2 ToUv(Vector3 v)
        {
            var vt = Vector3.Transform(v, TexScaleTransform);
            return new Vector2(vt.X, vt.Y);
        }       
    }
}
