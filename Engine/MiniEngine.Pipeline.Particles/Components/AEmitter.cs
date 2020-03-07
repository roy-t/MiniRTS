using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Utilities;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Particles.Components
{
    public abstract class AEmitter : IPhysicalComponent
    {
        private static readonly Random Random = new Random();

        private Seconds timeToSpawn;
        private Vector4 tint;

        public AEmitter(Entity entity, Vector3 position, Texture2D texture, int rows, int columns, float scale)
        {
            this.Entity = entity;
            this.Position = position;
            this.Texture = texture;
            this.Rows = rows;
            this.Columns = columns;

            this.Particles = new List<Particle>();

            this.SpawnInterval = 0.05f;
            this.timeToSpawn = 0.0f;

            this.Scale = scale;
            this.Direction = Vector3.Up;
            this.Speed = 3.0f;
            this.Spread = 0.5f;
            this.TimeToLive = 2.0f;
            this.TimePerFrame = 0.125f;
            this.Tint = Color.White;
        }

        public Entity Entity { get; }

        [Editor(nameof(Position))]
        public Vector3 Position { get; set; }

        public IconType Icon => IconType.Emitter;

        public Vector3[] Corners => new Vector3[] { this.Position, this.Position, this.Position, this.Position, this.Position, this.Position, this.Position, this.Position };

        [Editor(nameof(Texture))]
        public Texture2D Texture { get; }

        [Editor(nameof(Rows))]
        public int Rows { get; }

        [Editor(nameof(Columns))]
        public int Columns { get; }

        public List<Particle> Particles { get; }

        [Editor(nameof(SpawnInterval), nameof(SpawnInterval), 0, float.MaxValue)]
        public Seconds SpawnInterval { get; set; }

        [Editor(nameof(Scale), nameof(Scale), 0, float.MaxValue)]
        public float Scale { get; set; }

        [Editor(nameof(Direction), nameof(Direction), -1, 1)]
        public Vector3 Direction { get; set; }

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

        public void Update(Seconds elapsed)
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
            if (this.timeToSpawn <= 0.0f)
            {
                var velocity = Vector3.Normalize(this.Direction + this.GetSpreadVector()) * this.Speed;

                this.timeToSpawn += this.SpawnInterval;
                this.Particles.Add(
                    new Particle(
                        this.Position,
                        this.Scale,
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
