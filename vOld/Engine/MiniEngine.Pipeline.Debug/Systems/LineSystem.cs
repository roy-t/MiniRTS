using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Debug.Systems
{
    public sealed class LineSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly IComponentContainer<DebugLine> Components;
        private readonly ColorEffect Effect;

        private VertexPosition[] vertices;
        private short[] indices;

        public LineSystem(GraphicsDevice device, EffectFactory effectFactory, IComponentContainer<DebugLine> components)
        {
            this.Device = device;
            this.Components = components;

            this.Effect = effectFactory.Construct<ColorEffect>();

            this.vertices = new VertexPosition[0];
            this.indices = new short[0];
        }

        public void RenderLines(PerspectiveCamera viewPoint, GBuffer gBuffer)
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
                this.Effect.Color = component.Color;
                this.Effect.VisibleTint = component.VisibleTint;
                this.Effect.ClippedTint = component.ClippedTint;

                this.Effect.Apply(ColorEffectTechniques.ColorGeometryDepthTest);
                this.RenderLinePrimitive(component.Positions);
            }
        }

        private void RenderLinePrimitive(IReadOnlyList<Vector3> positions)
        {
            if (positions.Count < 2)
            {
                return;
            }

            if (this.vertices.Length < positions.Count)
            {
                this.vertices = new VertexPosition[positions.Count];
                this.indices = new short[positions.Count];
            }

            for (short i = 0; i < positions.Count; i++)
            {
                this.vertices[i].Position = positions[i];
                this.indices[i] = i;
            }

            this.Device.DrawUserIndexedPrimitives(PrimitiveType.LineStrip, this.vertices, 0, positions.Count, this.indices, 0, positions.Count - 1);
        }
    }
}
