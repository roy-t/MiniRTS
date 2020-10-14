using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Lighting
{
    public sealed class AmbientLightComponent : AComponent
    {
        public AmbientLightComponent(Entity entity, Color diffuseLight, float specularLight = 0.0f)
            : base(entity)
        {
            this.DiffuseLight = diffuseLight;
            this.SpecularLight = specularLight;
        }

        public Color DiffuseLight { get; set; }

        public float SpecularLight { get; set; }
    }
}
