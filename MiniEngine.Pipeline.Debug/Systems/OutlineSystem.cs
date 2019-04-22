using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Debug.Systems
{
    public sealed class OutlineSystem : ISystem
    {
        private readonly List<Outline> Outlines;
        private readonly GraphicsDevice Device;
        private readonly EntityLinker Linker;

        private readonly ColorEffect Effect;
        private readonly BoundsDrawer2D Quad;
        private readonly BoundsDrawer3D Cube;

        public OutlineSystem(GraphicsDevice device, ColorEffect effect, EntityLinker entityLinker)
        {
            this.Outlines = new List<Outline>();
            this.Device = device;
            this.Linker = entityLinker;

            this.Effect = effect;
            
            this.Cube = new BoundsDrawer3D(device);
            this.Quad = new BoundsDrawer2D(device);
        }

        public void Render3DOverlay(PerspectiveCamera viewPoint)
        {
            this.Outlines.Clear();
            this.Linker.GetComponents(this.Outlines);

            this.Effect.World = Matrix.Identity;
            this.Effect.View = viewPoint.View;
            this.Effect.Projection = viewPoint.Projection;

            this.Device.WireFrameState();

            for (var iOutline = 0; iOutline < this.Outlines.Count; iOutline++)
            {
                var outline = this.Outlines[iOutline];

                if (viewPoint.Frustum.Intersects(outline.Model.BoundingBox))
                {
                    this.Effect.Color = outline.Color3D;
                    this.Effect.Apply();
                    this.Cube.RenderOutline(outline.Model.BoundingBox);
                }
            }
        }

        public void Render2DOverlay(PerspectiveCamera viewPoint)
        {
            this.Outlines.Clear();
            this.Linker.GetComponents(this.Outlines);

            this.Effect.World      = Matrix.Identity;
            this.Effect.View       = Matrix.Identity;
            this.Effect.Projection = Matrix.Identity;

            this.Device.WireFrameState();

            for (var iOutline = 0; iOutline < this.Outlines.Count; iOutline++)
            {
                var outline = this.Outlines[iOutline];

                if (viewPoint.Frustum.Intersects(outline.Model.BoundingBox))
                {                    
                    this.Effect.Color = outline.Color2D;
                    this.Effect.Apply();

                    this.Quad.RenderOutline(outline.Model.BoundingBox, viewPoint);
                }
            }
        }                
    }
}
