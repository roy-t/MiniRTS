using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Effects;
using MiniEngine.Systems;
using MiniEngine.Primitives.VertexTypes;
using MiniEngine.Primitives.Bounds;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Models.Utilities;

namespace MiniEngine.Rendering.Systems
{
    public sealed class DebugRenderSystem : ISystem
    {
        private readonly GraphicsDevice Device;

        private readonly short[] Indices;
        private readonly Dictionary<Entity, BoundingBox> Models;
        private readonly RenderEffect RenderEffect;
        private readonly GBufferVertex[] Vertices;

        private readonly Texture2D BlueOutlineTexture;
        private readonly Texture2D RedOutlineTexture;


        public DebugRenderSystem(GraphicsDevice device, RenderEffect effect)
        {
            this.Device = device;
            this.RenderEffect = effect;
            this.Models = new Dictionary<Entity, BoundingBox>();

            this.BlueOutlineTexture = this.CreateTexture(Color.CornflowerBlue);
            this.RedOutlineTexture = this.CreateTexture(Color.Red);

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

        public bool Contains(Entity entity) => this.Models.ContainsKey(entity);

        public string Describe(Entity entity) => "debug";

        public void Remove(Entity entity) => this.Models.Remove(entity);

        public void Add(Entity entity, Model model, Matrix pose)
        {
            var boundingBox = model.ComputeBoundingBox(pose);
            this.Models.Add(entity, boundingBox);
        }

        public void Render3DOverlay(IViewPoint viewPoint)
        {
            this.RenderEffect.World = Matrix.Identity;
            this.RenderEffect.View = viewPoint.View;
            this.RenderEffect.Projection = viewPoint.Projection;
            this.RenderEffect.DiffuseMap = this.BlueOutlineTexture;

            using (this.Device.WireFrameState())
            {
                this.RenderEffect.Apply(RenderEffectTechniques.Textured);

                foreach (var bounds in this.Models.Values)
                {
                    var corners = bounds.GetCorners();
                    for (var i = 0; i < corners.Length; i++)
                    {
                        this.Vertices[i].Position = new Vector4(corners[i], 1);
                    }

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
            this.RenderEffect.World = Matrix.Identity;
            this.RenderEffect.View = Matrix.Identity;
            this.RenderEffect.Projection = Matrix.Identity;
            this.RenderEffect.DiffuseMap = this.RedOutlineTexture;

            using (this.Device.PostProcessState())
            {
                this.RenderEffect.Apply(RenderEffectTechniques.Textured);

                foreach (var bounds in this.Models.Values)
                {
                    var rect = BoundingRectangle.CreateFromProjectedBoundingBox(bounds, viewPoint.Frustum.Matrix);
                    var projectedCorners = rect.GetCorners();

                    for (var i = 0; i < projectedCorners.Length; i++)
                    {
                        this.Vertices[i].Position = new Vector4(projectedCorners[i].X, projectedCorners[i].Y, 0, 1);
                    }

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

        private Texture2D CreateTexture(Color color)
        {
            var texture = new Texture2D(this.Device, 1, 1);
            texture.SetData(new[] { color });

            return texture;
        }
    }
}