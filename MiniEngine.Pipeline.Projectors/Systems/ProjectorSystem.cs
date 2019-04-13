using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Effects.DeviceStates;
using System.Collections.Generic;
using MiniEngine.Effects;
using Microsoft.Xna.Framework;
using MiniEngine.Primitives.Bounds;
using System;
using MiniEngine.Effects.Techniques;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class ProjectorSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FullScreenQuad Quad;

        private readonly EntityLinker EntityLinker;
        private readonly ProjectorEffect Effect;
        private readonly List<Projector> Projectors;

        private readonly Vector3[] FrustumCorners;        

        public ProjectorSystem(GraphicsDevice device, EntityLinker entityLinker, ProjectorEffect effect)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
            this.Effect = effect;

            this.Quad = new FullScreenQuad(device);
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

                    
                    // TODO: the below code works, but is super inefficient, OPTIMIZE!
                    // It is probably also possible to use the trick below for all lights
                    // so we might want to separate it and built it into the Quad (which is then always used for these kind of effects
                    // like shadow casting lights, and sunlights, which is nice because those are the most expensive ones)
                    // Create a scene to test if it is actually faster and if it is possible to do without creating a million garbage

                    // TODO: maybe we can even do this work in the vertex shader?
                    projector.ViewPoint.Frustum.GetCorners(this.FrustumCorners);

                    var bounds = BoundingBox.CreateFromPoints(this.FrustumCorners);
                    var rect = BoundingRectangle.CreateFromProjectedBoundingBox(bounds, perspectiveCamera.ViewProjection);

                    var projectedCorners = rect.GetCorners();

                    var vertices = new Vector3[projectedCorners.Length];
                    for (var iCorner = 0; iCorner < projectedCorners.Length; iCorner++)
                    {
                        vertices[iCorner] = new Vector3(projectedCorners[iCorner].X, projectedCorners[iCorner].Y, 0);
                    }


                    var uv = new Vector2[4];                    

                    uv[0] = ToUv(vertices[0]);
                    uv[1] = ToUv(vertices[1]);
                    uv[2] = ToUv(vertices[2]);
                    uv[3] = ToUv(vertices[3]);


                    this.Effect.Apply(this.Technique);
                    this.Quad.Render(vertices[0], vertices[1], vertices[2], vertices[3],
                        uv[0], uv[1], uv[2], uv[3]);                   
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
