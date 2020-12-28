using System;

namespace MiniEngine.Systems
{
    public readonly struct Entity : IEquatable<Entity>
    {
        public static readonly Entity Zero = new Entity(0);

        public Entity(int id)
        {
            this.Id = id;
        }

        public int Id { get; }

        public bool Equals(Entity other) => this.Id == other.Id;

        public override int GetHashCode() => this.Id.GetHashCode();

        public override string ToString() => $"{this.Id}";

        public override bool Equals(object? obj)
            => obj is Entity entity && this.Equals(entity);

        public static bool operator ==(Entity left, Entity right)
            => left.Equals(right);

        public static bool operator !=(Entity left, Entity right)
            => !(left == right);
    }
}
