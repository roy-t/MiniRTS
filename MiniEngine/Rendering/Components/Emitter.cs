using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Components
{
    public sealed class Emitter
    {
        private static readonly Random Random = new Random();

        private Seconds timeToSpawn;

        public Emitter(Vector3 position, Texture2D texture, int rows, int columns)
        {
            this.Position = position;
            this.Texture = texture;
            this.Rows = rows;
            this.Columns = columns;

            this.Particles = new List<Particle>();

            this.SpawnInterval = 0.01f;
            this.timeToSpawn = 0.0f;

            this.Scale = 0.5f;
            this.Direction = Vector3.Up;
            this.Speed = 3.0f;
            this.Spread = 0.5f;
            this.TimeToLive = 2.0f;
            this.TimePerFrame = 0.125f;
        }

        public Vector3 Position { get; }
        public Texture2D Texture { get; }
        public int Rows { get; }
        public int Columns { get; }
        public List<Particle> Particles { get; }

        private Seconds SpawnInterval { get; }

        public float Scale { get; }
        public Vector3 Direction { get; }
        public float Speed { get; }
        public float Spread { get; }
        public Seconds TimeToLive { get; }
        public Seconds TimePerFrame { get; }

       
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
                }
            }

            this.timeToSpawn -= elapsed;
            if (this.timeToSpawn <= 0.0f)
            {                
                var velocity = Vector3.Normalize(this.Direction + GetSpreadVector()) * this.Speed;

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

        private static float GetRandomOffset()
        {
            return (((float)Random.NextDouble()) * 2.0f) - 1.0f;
        }
    }
}
