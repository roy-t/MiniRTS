using System;

namespace MiniEngine.Pipeline
{
    public sealed class Pass : IEquatable<Pass>
    {
        public Pass(PassType type, int iteration)
        {
            this.Type = type;
            this.Iteration = iteration;
        }

        public PassType Type { get; }
        public int Iteration { get; }

        public override bool Equals(object obj) => this.Equals(obj as Pass);
        public bool Equals(Pass other) => other != null && other.Type == this.Type && other.Iteration == this.Iteration;
        public override int GetHashCode() => ((int)this.Type * 1000) + this.Iteration;
    }
}
