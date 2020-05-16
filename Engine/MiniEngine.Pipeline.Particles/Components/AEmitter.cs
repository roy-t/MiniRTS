using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Utilities;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Particles.Components
{
    // TODO: use Pose 
    public abstract class AEmitter : IComponent
    {
        private static readonly Random Random = new Random();

        private Seconds timeToSpawn;
        private Vector4 tint;

        public AEmitter(Entity entity, Texture2D texture, int rows, int columns)
        {
            this.Entity = entity;
            this.Enabled = true;
            this.Texture = texture;
            this.Rows = rows;
            this.Columns = columns;

            this.Particles = new List<Particle>();

            this.SpawnInterval = 0.05f;
            this.timeToSpawn = 0.0f;

            this.Speed = 3.0f;
            this.Spread = 0.5f;
            this.TimeToLive = 2.0f;
            this.TimePerFrame = 0.125f;
            this.Tint = Color.White;
        }

        public Entity Entity { get; }

        [Editor(nameof(Enabled))]
        public bool Enabled { get; set; }

        [Editor(nameof(Texture))]
        public Texture2D Texture { get; }

        [Editor(nameof(Rows))]
        public int Rows { get; }

        [Editor(nameof(Columns))]
        public int Columns { get; }

        public List<Particle> Particles { get; }

        [Editor(nameof(SpawnInterval), nameof(SpawnInterval), 0, float.MaxValue)]
        public Seconds SpawnInterval { get; set; }

        [Editor(nameof(Speed), nameof(Speed), 0, float.MaxValue)]
        public float Speed { get; set; }

        [Editor(nameof(Spread), nameof(Spread), 0, 1)]
        public float Spread { get; set; }

        [Editor(nameof(TimeToLive), nameof(TimeToLive), 0, float.MaxValue)]
        public Seconds TimeToLive { get; set; }

        [Editor(nameof(TimePerFrame), nameof(TimePerFrame), 0, float.MaxValue)]
        public Seconds TimePerFrame { get; set; }

        [Editor(nameof(Tint))]
        public Color Tint
        {
            get => new Color(this.tint);
            set => this.tint = value.ToVector4();
        }

        public void Update(Seconds elapsed, Pose pose)
        {
            for (var i = this.Particles.Count - 1; i >= 0; i--)
            {
                var particle = this.Particles[i];
                particle.LifeTime += elapsed;
                if (particle.LifeTime >= this.TimeToLive)
                {
                    this.Particles.RemoveAt(i);
                }
                else
                {
                    particle.Position += particle.LinearVelocity * elapsed.Value;
                    particle.Frame = (int)(particle.LifeTime.Value / particle.TimePerFrame.Value);

                    var progress = particle.LifeTime / this.TimeToLive;
                    var lifeTimeRatio = 1.0f - Easings.ExponentialEaseIn(progress);
                    particle.Tint = new Color(this.tint.X * lifeTimeRatio, this.tint.Y * lifeTimeRatio, this.tint.Z * lifeTimeRatio, this.tint.W * lifeTimeRatio);
                }
            }

            this.timeToSpawn -= elapsed;
            if (this.Enabled && this.timeToSpawn <= 0.0f)
            {
                var direction = Vector3.TransformNormal(Vector3.Forward, pose.RotationMatrix);

                var velocity = Vector3.Normalize(direction + this.GetSpreadVector()) * this.Speed;

                this.timeToSpawn += this.SpawnInterval;
                this.Particles.Add(
                    new Particle(
                        pose.Position,
                        pose.Scale,
                        velocity,
                        this.TimePerFrame));
            }
        }

        private Vector3 GetSpreadVector()
        {
            var x = GetRandomOffset() * this.Spread;
            var y = GetRandomOffset() * this.Spread;
            var z = GetRandomOffset() * this.Spread;

            return new Vector3(x, y, z);
        }

        private static float GetRandomOffset() => ((float)Random.NextDouble() * 2.0f) - 1.0f;
    }
}
