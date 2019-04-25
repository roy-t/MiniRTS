using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;

namespace MiniEngine.Pipeline.Debug.Systems
{
    public sealed class IconSystem : DebugSystem
    {
        private const float Scale = 0.05f;

        private readonly GraphicsDevice Device;
        private readonly TextureEffect Effect;
        private readonly IconLibrary Library;
        private readonly UnitQuad Quad;

        public IconSystem(GraphicsDevice device, TextureEffect effect, EntityLinker entityLinker, IconLibrary library)
            : base(entityLinker)
        {
            this.Device = device;
            this.Effect = effect;
            this.Library = library;
            this.Quad = new UnitQuad(device);
        }

        public void RenderIcons(PerspectiveCamera viewPoint)
        {
            this.Effect.World      = Matrix.Identity;
            this.Effect.View       = Matrix.Identity;
            this.Effect.Projection = Matrix.Identity;

            this.Device.PostProcessState();

            foreach ((var entity, var component, var info, var property, var attribute) in this.EnumerateAttributes<IconAttribute>())
            {                
                var position = (Vector3)property.GetGetMethod().Invoke(component, null);
                
                if(viewPoint.Frustum.Contains(position) != ContainmentType.Disjoint)
                {
                    var screenPosition = ProjectionMath.WorldToView(position, viewPoint.ViewProjection);

                    this.Effect.World = Matrix.CreateScale(new Vector3(Scale / viewPoint.AspectRatio, Scale, Scale)) * Matrix.CreateTranslation(new Vector3(screenPosition, 0));
                    this.Effect.Texture = this.Library.GetIcon(attribute.Type);
                    this.Effect.Apply();

                    this.Quad.Render();
                }                
            }
        }
    }
}
