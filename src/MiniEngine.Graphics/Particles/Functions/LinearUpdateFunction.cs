using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Particles.Functions
{
    public sealed class LinearUpdateFunction : IParticleUpdateFunction
    {
        public LinearUpdateFunction()
        {
            this.StartVelocity = 1.0f;
            this.EndVelocity = 1.0f;
            this.StartColor = Color.White;
            this.EndColor = Color.White;
        }

        public float StartVelocity { get; set; }

        public float EndVelocity { get; set; }

        public Color StartColor { get; set; }

        public Color EndColor { get; set; }

        public void Update(float elapsed, Matrix transform, ref Particle particle)
        {
            var velocity = MathHelper.Lerp(this.EndVelocity, this.StartVelocity, particle.Energy);

            particle.Position += transform.Forward * velocity * elapsed;
            particle.Color = Color.Lerp(this.EndColor, this.StartColor, particle.Energy);

        }
    }
}
