using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class AmbientLightSystem : ISystem
    {
        private readonly EntityLinker EntityLinker;
        private readonly List<AmbientLight> Lights;
        
        public AmbientLightSystem(EntityLinker entityLinker)
        {
            this.Lights = new List<AmbientLight>();
            this.EntityLinker = entityLinker;
        }
               
        public Color ComputeAmbientLightZeroAlpha()
        {
            this.Lights.Clear();
            this.EntityLinker.GetComponents(this.Lights);

            var accumulate = Color.TransparentBlack;
            foreach (var light in this.Lights)
            {
                accumulate.R += light.Color.R;
                accumulate.G += light.Color.G;
                accumulate.B += light.Color.B;
            }

            return accumulate;
        }
    }
}