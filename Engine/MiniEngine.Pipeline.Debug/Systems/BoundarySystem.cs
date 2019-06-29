using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Debug.Systems
{
    public sealed class BoundarySystem : DebugSystem
    {
        private readonly GraphicsDevice Device;

        private readonly ColorEffect Effect;
        private readonly BoundsDrawer2D Quad;
        private readonly BoundsDrawer3D Bounds;

        public BoundarySystem(GraphicsDevice device, ColorEffect effect, IComponentContainer<DebugInfo> debugInfos, IList<IComponentContainer> containers)
            : base(debugInfos, containers)
        {
            this.Device = device;
            this.Effect = effect;
            
            this.Bounds = new BoundsDrawer3D(device);
            this.Quad = new BoundsDrawer2D(device);
        }

        public void Render3DOverlay(PerspectiveCamera viewPoint, GBuffer gBuffer)
        {           
            this.Effect.World      = Matrix.Identity;
            this.Effect.View       = viewPoint.View;
            this.Effect.Projection = viewPoint.Projection;

            this.Effect.DepthMap              = gBuffer.DepthTarget;
            this.Effect.CameraPosition        = viewPoint.Position;
            this.Effect.InverseViewProjection = viewPoint.InverseViewProjection;

            this.Device.PostProcessState();

            foreach ((var entity, var component, var info, var property, var attribute) in this.EnumerateAttributes<BoundaryAttribute>())
            {
                this.Effect.Color                 = info.Color3D;                
                this.Effect.VisibleTint           = info.VisibileIconTint;
                this.Effect.ClippedTint           = info.ClippedIconTint;

                this.Effect.Apply(ColorEffectTechniques.ColorGeometryDepthTest);

                var boundary = property.GetGetMethod().Invoke(component, null);
                switch (attribute.Type)
                {
                    case BoundaryType.Frustum:
                        var frustum = (BoundingFrustum)boundary;
                        if (viewPoint.Frustum.Intersects(frustum))
                        {
                            this.Bounds.RenderOutline(frustum);
                        }
                        break;
                    case BoundaryType.BoundingBox:
                        var boundingBox = (BoundingBox)boundary;
                        if (viewPoint.Frustum.Intersects(boundingBox))
                        {
                            this.Bounds.RenderOutline(boundingBox);
                        }
                        break;
                }
            }
        }

        public void Render2DOverlay(PerspectiveCamera viewPoint, GBuffer gBuffer)
        {
            this.Effect.World      = Matrix.Identity;
            this.Effect.View       = Matrix.Identity;
            this.Effect.Projection = Matrix.Identity;

            this.Effect.DepthMap              = gBuffer.DepthTarget;
            this.Effect.CameraPosition        = viewPoint.Position;
            this.Effect.InverseViewProjection = viewPoint.InverseViewProjection;

            this.Device.PostProcessState();

            foreach ((var entity, var component, var info, var property, var attribute) in this.EnumerateAttributes<BoundaryAttribute>())
            {
                this.Effect.Color = info.Color2D;
                this.Effect.VisibleTint = info.VisibileIconTint;
                this.Effect.ClippedTint = info.ClippedIconTint;

                var boundary = property.GetGetMethod().Invoke(component, null);
                switch (attribute.Type)
                {
                    case BoundaryType.Frustum:
                        var frustum = (BoundingFrustum)boundary;                        
                        if (viewPoint.Frustum.Intersects(frustum))
                        {
                            this.Effect.WorldPosition = BoundingSphere.CreateFromFrustum(frustum).Center;
                            this.Effect.Apply(ColorEffectTechniques.ColorPointDepthTest);
                            this.Quad.RenderOutline(frustum, viewPoint);
                        }
                        break;
                    case BoundaryType.BoundingBox:
                        var boundingBox = (BoundingBox)boundary;
                        if (viewPoint.Frustum.Intersects(boundingBox))
                        {
                            this.Effect.WorldPosition = BoundingSphere.CreateFromBoundingBox(boundingBox).Center;
                            this.Effect.Apply(ColorEffectTechniques.ColorPointDepthTest);
                            this.Quad.RenderOutline(boundingBox, viewPoint);
                        }
                        break;
                }
            }
        }               
    }
}
