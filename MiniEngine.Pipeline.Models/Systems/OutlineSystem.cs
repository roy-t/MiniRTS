using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Bounds;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives.VertexTypes;
using MiniEngine.Systems;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Models.Systems
{
    public sealed class OutlineSystem : ISystem
    {
        private readonly List<Outline> Outlines;
        private readonly GraphicsDevice Device;
        private readonly EntityLinker Linker;

        private readonly Dictionary<Color, Texture2D> Colors;

        private readonly short[] Indices;
        private readonly GBufferVertex[] Vertices;
        private readonly RenderEffect RenderEffect;

        public OutlineSystem(GraphicsDevice device, RenderEffect effect, EntityLinker entityLinker)
        {
            this.Outlines = new List<Outline>();
            this.Device = device;
            this.Linker = entityLinker;

            this.Colors = new Dictionary<Color, Texture2D>();

            this.RenderEffect = effect;

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

        public void Render3DOverlay(IViewPoint viewPoint)
        {
            this.Outlines.Clear();
            this.Linker.GetComponents(this.Outlines);

            this.RenderEffect.World = Matrix.Identity;
            this.RenderEffect.View = viewPoint.View;
            this.RenderEffect.Projection = viewPoint.Projection;

            this.Device.WireFrameState();

            for (var iOutline = 0; iOutline < this.Outlines.Count; iOutline++)
            {
                var outline = this.Outlines[iOutline];
                if (outline.Color3D.A > 0)
                {
                    var corners = outline.Model.BoundingBox.GetCorners();
                    for (var iCorner = 0; iCorner < corners.Length; iCorner++)
                    {
                        this.Vertices[iCorner].Position = new Vector4(corners[iCorner], 1);
                    }

                    this.RenderEffect.DiffuseMap = GetTexture(outline.Color3D);
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
            }
        }

        public void Render2DOverlay(IViewPoint viewPoint)
        {
            this.Outlines.Clear();
            this.Linker.GetComponents(this.Outlines);

            this.RenderEffect.World = Matrix.Identity;
            this.RenderEffect.View = Matrix.Identity;
            this.RenderEffect.Projection = Matrix.Identity;

            this.Device.PostProcessState();

            for (var iOutline = 0; iOutline < this.Outlines.Count; iOutline++)
            {
                var outline = this.Outlines[iOutline];

                if (outline.Color2D.A > 0)
                {
                    var rect = BoundingRectangle.CreateFromProjectedBoundingBox(outline.Model.BoundingBox, viewPoint.Frustum.Matrix);
                    var projectedCorners = rect.GetCorners();

                    for (var iCorner = 0; iCorner < projectedCorners.Length; iCorner++)
                    {
                        this.Vertices[iCorner].Position = new Vector4(projectedCorners[iCorner].X, projectedCorners[iCorner].Y, 0, 1);
                    }

                    this.RenderEffect.DiffuseMap = GetTexture(outline.Color2D);
                    this.RenderEffect.Apply(RenderEffectTechniques.Textured);

                    this.Device.DrawUserIndexedPrimitives(
                        PrimitiveType.LineList,
                        this.Vertices,
                        0,
                        4,
                        this.Indices,
                        0,
                        4);
                }
            }
        }

        private Texture2D GetTexture(Color color)
        {
            if(this.Colors.TryGetValue(color, out var texture))
            {
                return texture;
            }

            texture = this.CreateTexture(color);
            this.Colors.Add(color, texture);

            return texture;
        }

        private Texture2D CreateTexture(Color color)
        {
            var texture = new Texture2D(this.Device, 1, 1);
            texture.SetData(new[] { color });

            return texture;
        }
    }
}
