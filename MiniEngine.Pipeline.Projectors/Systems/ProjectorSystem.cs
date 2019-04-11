using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Effects.DeviceStates;
using System.Collections.Generic;
using MiniEngine.Effects;
using Microsoft.Xna.Framework;

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

            this.Projectors = new List<Projector>();

            this.FrustumCorners = new Vector3[8];
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


                    projector.ViewPoint.Frustum.GetCorners(this.FrustumCorners);
                  
                    // TODO: very close but
                    // - When moving the camera the projection 'moves' in the wrong direction
                    // - The UV coordinates are not yet computed
                    // - Seems like a lot of operations to get things done...

                    for(var iCorner = 0; iCorner < 8; iCorner++)
                    {
                        this.FrustumCorners[iCorner] = Vector3.Transform(this.FrustumCorners[iCorner], perspectiveCamera.ViewProjection);                        
                    }

                    var minX = MathHelper.Min(this.FrustumCorners[0].X, MathHelper.Min(this.FrustumCorners[1].X, MathHelper.Min(this.FrustumCorners[2].X, this.FrustumCorners[3].X)));
                    var minY = MathHelper.Min(this.FrustumCorners[0].Y, MathHelper.Min(this.FrustumCorners[1].Y, MathHelper.Min(this.FrustumCorners[2].Y, this.FrustumCorners[3].Y)));

                    var maxX = MathHelper.Max(this.FrustumCorners[0].X, MathHelper.Max(this.FrustumCorners[1].X, MathHelper.Max(this.FrustumCorners[2].X, this.FrustumCorners[3].X)));
                    var maxY = MathHelper.Max(this.FrustumCorners[0].Y, MathHelper.Max(this.FrustumCorners[1].Y, MathHelper.Max(this.FrustumCorners[2].Y, this.FrustumCorners[3].Y)));


                    var v0 = new Vector3(maxX, minY, 0);
                    var v1 = new Vector3(minX, minY, 0);
                    var v2 = new Vector3(minX, maxY, 0);
                    var v3 = new Vector3(maxX, maxY, 0);

                    
                    this.Quad.Render(v0, v1, v2, v3);
                    //this.Quad.Render();
                }
            }
        }

        private const int NearLeftTop = 0;
        private const int NearRightTop = 1;
        private const int NearRightBottom = 2;
        private const int NearLeftBottom = 3;
        private const int FarLeftTop = 4;
        private const int FarRightTop = 5;
        private const int FarRightBottom = 6;
        private const int FarLeftBottom = 7;        

    }
}
