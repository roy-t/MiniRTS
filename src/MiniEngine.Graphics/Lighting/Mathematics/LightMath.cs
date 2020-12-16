using System;

namespace MiniEngine.Graphics.Lighting.Mathematics
{
    public static class LightMath
    {
        private const float MinimumLightInfluence = 0.001f;

        public static float LightVolumeRadius(float strength)
            => MathF.Sqrt(strength / MinimumLightInfluence);

        public static float Attenuation(float strength, float distance)
            => strength / (distance * distance);
    }
}
