using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Components
{
    public sealed class Emitter
    {        
        public Emitter(Vector3 position, Texture2D texture, int rows, int columns)
        {
            this.Position = position;
            this.Texture = texture;
            this.Rows = rows;
            this.Columns = columns;

            this.Particles = new List<Particle>();
            // TODO: actually spawn particles and stuff
            this.Particles.Add(new Particle());
        }

        public Vector3 Position { get; }
        public Texture2D Texture { get; }
        public int Rows { get; }
        public int Columns { get; }
        public List<Particle> Particles { get; }       
    }
}
