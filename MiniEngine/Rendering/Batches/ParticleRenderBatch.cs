using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Batches
{
    public sealed class ParticleRenderBatch
    {
        private readonly IReadOnlyList<ParticlePose> Particles;
        private readonly IViewPoint ViewPoint;
        private readonly Quad Quad;
        private readonly RenderEffect Effect;    

        private readonly Texture2D NullMask;
        private readonly Texture2D NullNormalMap;
        private readonly Texture2D NullSpecularMap;

        public ParticleRenderBatch(Quad quad, RenderEffect effect,
            IReadOnlyList<ParticlePose> particles,
            IViewPoint viewPoint,
            Texture2D nullMask,
            Texture2D nullNormalMap,
            Texture2D nullSpecularMap)
        {
            this.Quad = quad;
            this.Effect = effect;
            this.Particles = particles;
            this.ViewPoint = viewPoint;
            this.NullMask = nullMask;
            this.NullNormalMap = nullNormalMap;
            this.NullSpecularMap = nullSpecularMap;
        }

        public void Draw(Techniques technique)
        {
            this.Effect.Mask = this.NullMask;
            this.Effect.NormalMap = this.NullNormalMap;
            this.Effect.SpecularMap = this.NullSpecularMap;

            this.Effect.Projection = this.ViewPoint.Projection;
            this.Effect.View = this.ViewPoint.View;

            foreach (var particle in this.Particles)
            {
                this.Effect.DiffuseMap = particle.Texture;
                
                this.Effect.World = particle.Pose;
                this.Effect.Apply(Techniques.MRT);

                this.Quad.SetTextureCoordinates(particle.MinUv, particle.MaxUv);
                this.Quad.Render();
            }
        }        
    }
}
