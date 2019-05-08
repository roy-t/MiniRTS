using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class DynamicTextureSystem : IUpdatableSystem
    {
        private readonly GraphicsDevice Device;
        private readonly EntityLinker EntityLinker;

        private readonly List<DynamicTexture> DynamicTextures;

        public DynamicTextureSystem(GraphicsDevice device, EntityLinker entityLinker)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
            this.DynamicTextures = new List<DynamicTexture>();
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            this.DynamicTextures.Clear();
            this.EntityLinker.GetComponents(this.DynamicTextures);

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
