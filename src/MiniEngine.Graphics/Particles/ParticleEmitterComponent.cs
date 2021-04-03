using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleEmitterComponent : AComponent, IDisposable
    {
        private static readonly Random R = new();

        private bool isEnabled;

        public ParticleEmitterComponent(Entity entity, GraphicsDevice device, int count)
            : base(entity)
        {
            var dimensions = (int)Math.Ceiling(Math.Sqrt(count));
            this.Count = dimensions * dimensions;

            this.Velocity = new DoubleBufferedRenderTarget(device, dimensions, SurfaceFormat.HalfVector4);
            this.Position = new DoubleBufferedRenderTarget(device, dimensions, SurfaceFormat.Vector4);
            this.Forces = new DoubleBufferedRenderTarget(device, dimensions, SurfaceFormat.Vector4);

            var instances = new Particle[this.Count];
            var i = 0;
            for (var x = 0; x < dimensions; x++)
            {
                for (var y = 0; y < dimensions; y++)
                {
                    var u = x / (float)dimensions;
                    var v = y / (float)dimensions;

                    instances[i++] = new Particle(new Vector2(u, v));
                }
            }
            this.Instances = new VertexBuffer(device, PointVertex.Declaration, count, BufferUsage.WriteOnly);
            this.Instances.SetData(instances);

            this.SeedData();

            this.isEnabled = true;
        }

        public bool IsEnabled
        {
            get => this.isEnabled;
            set
            {
                if (value == false)
                {
                    this.SeedData();
                }
                this.isEnabled = value;
            }
        }

        public float Metalicness { get; set; } = 0.0f;
        public float Roughness { get; set; } = 1.0f;

        /// <summary>
        /// The color a slow moving particle will have
        /// </summary>
        public Color SlowColor { get; set; } = new Color(1.0f, 0.8f, 0.1f, 1.0f);

        /// <summary>
        /// The color a fast moving particle will have
        /// </summary>
        public Color FastColor { get; set; } = new Color(1.0f, 0.0f, 0.0f, 1.0f);

        /// <summary>
        /// Multiple of the velocity of the particle when determining the particle color
        /// </summary>
        public float ColorVelocityModifier { get; set; } = 0.2f;

        /// <summary>
        /// Diameter of circle where particles spawn
        /// </summary>
        public float Size { get; set; } = 0.3f;

        /// <summary>
        /// Maximum seconds before particles expire and respawn
        /// </summary>
        public float MaxLifeTime { get; set; } = 1.5f;

        /// <summary>
        /// Modifies the underlying size of the potential field, a smaller number leads to more detail
        /// while a larger number provides a smoother flow.
        /// </summary>
        public float LengthScale { get; set; } = 0.5f;

        /// <summary>
        /// Average speed at which particles travel through the field
        /// </summary>
        public float FieldSpeed { get; set; } = 1.0f;

        /// <summary>
        /// Strength of the underlying simplex noise
        /// </summary>
        public float NoiseStrength { get; set; } = 0.4f;

        /// <summary>
        /// Speed at which the underlyng potential field changes
        /// </summary>
        public float ProgressionRate { get; set; } = 0.5f;

        /// <summary>
        /// Position of a sphere that particles try to evade, interactions of particles
        /// with this sphere leads to turbulence.
        /// </summary>
        public Vector3 SpherePosition { get; set; } = Vector3.Forward;

        /// <summary>
        /// Radius of the sphere
        /// </summary>
        public float SphereRadius { get; set; } = 0.5f;

        public int Count { get; }

        public VertexBuffer Instances { get; }

        public DoubleBufferedRenderTarget Velocity { get; }
        public DoubleBufferedRenderTarget Position { get; }
        public DoubleBufferedRenderTarget Forces { get; }

        public void Swap()
        {
            this.Velocity.Swap();
            this.Position.Swap();
            this.Forces.Swap();
        }

        private void SeedData()
        {
            var data = new Vector4[this.Count];

            for (var i = 0; i < this.Count; i++)
            {
                var a = R.NextDouble() * MathHelper.TwoPi;
                var r = Math.Sqrt(R.NextDouble()) * this.Size;
                var x = r * Math.Cos(a);
                var y = r * Math.Sin(a);

                var age = (R.NextDouble() * this.MaxLifeTime) - this.MaxLifeTime;

                data[i] = new Vector4((float)x, (float)y, 0.0f, (float)age);

            }

            this.Position.WriteTarget.SetData(data);
            this.Swap();
        }

        public void Dispose()
        {
            this.Velocity.Dispose();
            this.Position.Dispose();
            this.Forces.Dispose();
        }
    }
}
