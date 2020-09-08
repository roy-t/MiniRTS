using System;
using System.Diagnostics.CodeAnalysis;

namespace MiniEngine.Systems
{
    public struct Entity : IEquatable<Entity>
    {
        public Entity(ushort id)
        {
            this.Id = id;
        }

        public ushort Id { get; }

        public bool Equals([AllowNull] Entity other) => this.Id == other.Id;

        public override int GetHashCode() => HashCode.Combine(this.Id);
    }
}
