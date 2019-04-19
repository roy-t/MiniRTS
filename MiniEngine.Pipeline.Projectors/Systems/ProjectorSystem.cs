using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives.VertexTypes;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class ProjectorSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly WrappableQuad Quad;

        private readonly EntityLinker EntityLinker;
        private readonly ProjectorEffect Effect;
        private readonly RenderEffect RenderEffect;
        private readonly List<Projector> Projectors;

        private readonly Vector3[] FrustumCorners;

        private readonly short[] Indices;
        private readonly GBufferVertex[] Vertices;

        public ProjectorSystem(GraphicsDevice device, EntityLinker entityLinker, ProjectorEffect effect, RenderEffect renderEffect)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
            this.Effect = effect;
            this.RenderEffect = renderEffect;

            this.Quad = new WrappableQuad(device);
            this.Technique = ProjectorEffectTechniques.Projector;

            this.Projectors = new List<Projector>();

            this.FrustumCorners = new Vector3[8];


            this.Vertices = new[]
            {
                // front
                new GBufferVertex(new Vector3(-1, 1, -1)), // tl
                new GBufferVertex(new Vector3(1, 1, -1)), // tr
                new GBufferVertex(new Vector3(-1, -1, -1)), // br
                new GBufferVertex(new Vector3(1, -1, -1)), // bl

                // back
                new GBufferVertex(new Vector3(-1, 1, 1)), // tl
                new GBufferVertex(new Vector3(1, 1, 1)), // tr
                new GBufferVertex(new Vector3(-1, -1, 1)), // br
                new GBufferVertex(new Vector3(1, -1, 1)) // bl
            };

            this.Indices = new short[]
            {
                0,
                1,
                1,
                2,
                2,
                3,
                3,
                0,
                4,
                5,
                5,
                6,
                6,
                7,
                7,
                4,
                0,
                4,
                1,
                5,
                2,
                6,
                3,
                7
            };
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
                    
                    this.Quad.WrapOnScreen(projector.ViewPoint.Frustum, perspectiveCamera);

                    this.Effect.Apply(this.Technique);
                    //this.Quad.Render();

                    this.Quad.Render(projector.ViewPoint.Frustum);
                    
                    this.DrawBoundingBox(projector.ViewPoint.Frustum, projector.Texture, perspectiveCamera);
                }
            }
        }

        // TODO: move the outline system and make sure it can draw projectors as well so that we don't need this code!
        private void DrawBoundingBox(BoundingFrustum frustum, Texture2D texture, PerspectiveCamera camera)
        {
            var corners = frustum.GetCorners();
            for (var iCorner = 0; iCorner < corners.Length; iCorner++)
            {
                this.Vertices[iCorner].Position = new Vector4(corners[iCorner], 1);
            }

            this.RenderEffect.World = Matrix.Identity;
            this.RenderEffect.View = camera.View;
            this.RenderEffect.Projection = camera.Projection;
            this.RenderEffect.DiffuseMap = texture;
            this.RenderEffect.Apply(RenderEffectTechniques.Textured);

            this.Device.DrawUserIndexedPrimitives(
                PrimitiveType.LineList,
                this.Vertices,
                0,
                this.Vertices.Length,
                this.Indices,
                0,
                12);
        }

        private static readonly Matrix TexScaleTransform = Matrix.CreateScale(0.5f, -0.5f, 1.0f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);
        private static Vector2 ToUv(Vector3 v)
        {
            var vt = Vector3.Transform(v, TexScaleTransform);
            return new Vector2(vt.X, vt.Y);
        }       
    }
}
