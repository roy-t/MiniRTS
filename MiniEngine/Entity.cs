using System;

namespace MiniEngine
{
    public struct Entity : IEquatable<Entity>
    {
        private readonly int Id;

        public Entity(int id)
        {
            this.Id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj is Entity)
            {
                return Equals((Entity) obj);
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
    }
}
