using System;

namespace MiniEngine.Systems
{
    public readonly struct Entity : IEquatable<Entity>
    {
        public Entity(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public string Name { get; }

        public override bool Equals(object obj)
        {
            if (obj is Entity entity)
            {
                return this.Equals(entity);
            }

            return false;
        }

        public int Id { get; }

        public static bool operator ==(Entity a, Entity b) => a.Equals(b);
        public static bool operator !=(Entity a, Entity b) => !a.Equals(b);

        public bool Equals(Entity other) => this.Id == other.Id;

        public override int GetHashCode() => this.Id;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                return $"Entity {this.Id}";
            }

            return $"{this.Name} ({this.Id})";
        }

        public static implicit operator int(Entity entity) => entity.Id;
    }
}
