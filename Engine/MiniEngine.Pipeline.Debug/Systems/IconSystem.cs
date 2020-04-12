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
    public sealed class IconSystem : ISystem
    {
        private const float IconScale = 0.05f;

        private readonly GraphicsDevice Device;
        private readonly IComponentContainer<DebugInfo> Components;
        private readonly IComponentContainer<Pose> Poses;
        private readonly TextureEffect Effect;
        private readonly IconLibrary Library;
        private readonly UnitQuad Quad;

        public IconSystem(GraphicsDevice device, EffectFactory effectFactory, IComponentContainer<DebugInfo> components, IComponentContainer<Pose> poses, IconLibrary library)
        {
            this.Device = device;
            this.Components = components;
            this.Poses = poses;
            this.Effect = effectFactory.Construct<TextureEffect>();
            this.Library = library;
            this.Quad = new UnitQuad(device);
        }

        public void RenderIcons(PerspectiveCamera viewPoint, GBuffer gBuffer)
        {
            this.Effect.World = Matrix.Identity;
            this.Effect.View = Matrix.Identity;
            this.Effect.Projection = Matrix.Identity;
            this.Effect.DepthMap = gBuffer.DepthTarget;

            this.Device.PostProcessState();

            for (var i = 0; i < this.Components.Count; i++)
            {
                var component = this.Components[i];
                var pose = this.Poses.Get(component.Entity);
                var position = pose.Position;

                if (viewPoint.Frustum.Contains(position) != ContainmentType.Disjoint)
                {
                    var screenPosition = ProjectionMath.WorldToView(position, viewPoint.ViewProjection);

                    this.Effect.World = Matrix.CreateScale(new Vector3(IconScale / viewPoint.AspectRatio, IconScale, IconScale)) * Matrix.CreateTranslation(new Vector3(screenPosition, 0));
                    this.Effect.Texture = this.Library.GetIcon(component.Icon);
                    this.Effect.WorldPosition = position;
                    this.Effect.CameraPosition = viewPoint.Position;
                    this.Effect.InverseViewProjection = viewPoint.InverseViewProjection;
                    this.Effect.VisibleTint = component.BoundaryVisibleTint;
                    this.Effect.ClippedTint = component.BoundaryClippedTint;

                    this.Effect.Apply(TextureEffectTechniques.TexturePointDepthTest);

                    this.Quad.Render();
                }
            }
        }
    }
}
