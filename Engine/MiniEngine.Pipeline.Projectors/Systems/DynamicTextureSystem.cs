using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class DynamicTextureSystem : IUpdatableSystem
    {
        private readonly GraphicsDevice Device;

        private readonly IComponentContainer<DynamicTexture> DynamicTextures;

        public DynamicTextureSystem(GraphicsDevice device, IComponentContainer<DynamicTexture> dynamicTexture)
        {
            this.Device = device;
            this.DynamicTextures = dynamicTexture;
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            for(var i = 0; i < this.DynamicTextures.Count; i++)
            {
                var dynamicTexture = this.DynamicTextures[i];

                var input = dynamicTexture.Input;
                input.Update(dynamicTexture.ViewPoint, elapsed, dynamicTexture.GBuffer, dynamicTexture.Pass);
                dynamicTexture.Pipeline.Execute(input, dynamicTexture.Label);
            }
        }
    }
}
