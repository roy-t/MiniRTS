using System;

namespace MiniEngine
{
    public readonly struct Entity : IEquatable<Entity>
    {
        private readonly int Id;

        public Entity(int id)
        {
            this.Id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj is Entity entity)
            {
                return Equals(entity);
            }

            return false;
        }

        public bool Equals(Entity other)
        {
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return this.Id;
        }

        public override string ToString() => $"Entity {this.Id}";        
    }
}
