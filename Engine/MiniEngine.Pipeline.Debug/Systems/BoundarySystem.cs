using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Debug.Systems
{
    public sealed class BoundarySystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly IComponentContainer<DebugInfo> Components;
        private readonly IComponentContainer<Bounds> Bounds;
        private readonly ColorEffect Effect;
        private readonly BoundsDrawer2D Quad;
        private readonly BoundsDrawer3D Bound;


        public BoundarySystem(GraphicsDevice device, EffectFactory effectFactory, IComponentContainer<DebugInfo> components, IComponentContainer<Bounds> bounds)
        {
            this.Device = device;
            this.Components = components;
            this.Bounds = bounds;
            this.Effect = effectFactory.Construct<ColorEffect>();
            this.Bound = new BoundsDrawer3D(device);
            this.Quad = new BoundsDrawer2D(device);
        }

        public void Render3DBounds(PerspectiveCamera viewPoint, GBuffer gBuffer)
        {
            this.Effect.World = Matrix.Identity;
            this.Effect.View = viewPoint.View;
            this.Effect.Projection = viewPoint.Projection;

            this.Effect.DepthMap = gBuffer.DepthTarget;
            this.Effect.CameraPosition = viewPoint.Position;
            this.Effect.InverseViewProjection = viewPoint.InverseViewProjection;

            this.Device.PostProcessState();

            for (var i = 0; i < this.Components.Count; i++)
            {
                var component = this.Components[i];
                var bounds = this.Bounds.Get(component.Entity);
                this.Effect.Color = component.Color3D;
                this.Effect.VisibleTint = component.BoundaryVisibleTint;
                this.Effect.ClippedTint = component.BoundaryClippedTint;

                this.Effect.Apply(ColorEffectTechniques.ColorGeometryDepthTest);
                this.Bound.RenderOutline(bounds.BoundingSphere);
            }
        }

        public void Render2DBounds(PerspectiveCamera viewPoint, GBuffer gBuffer)
        {
            this.Effect.World = Matrix.Identity;
            this.Effect.View = Matrix.Identity;
            this.Effect.Projection = Matrix.Identity;

            this.Effect.DepthMap = gBuffer.DepthTarget;
            this.Effect.CameraPosition = viewPoint.Position;
            this.Effect.InverseViewProjection = viewPoint.InverseViewProjection;

            this.Device.PostProcessState();

            for (var i = 0; i < this.Components.Count; i++)
            {
                var component = this.Components[i];
                var bounds = this.Bounds.Get(component.Entity);
                this.Effect.Color = component.Color2D;
                this.Effect.VisibleTint = component.BoundaryVisibleTint;
                this.Effect.ClippedTint = component.BoundaryClippedTint;

                this.Effect.Apply(ColorEffectTechniques.ColorGeometryDepthTest);
                this.Quad.RenderOutline(bounds.BoundingSphere, viewPoint);
            }
        }
    }
}
