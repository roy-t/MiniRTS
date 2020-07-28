using MiniEngine.Systems.Annotations;

namespace GameLogic.BluePrints
{
    public sealed class AsteroidBluePrint
    {
        public AsteroidBluePrint()
        {
            this.Radius = 1.0f;
            this.Subdivisions = 24;
        }

        [Editor(nameof(Radius))]
        public float Radius { get; set; }


        [Editor(nameof(Subdivisions), nameof(Subdivisions), 0.0f, 90.0f)]
        public int Subdivisions { get; set; }
    }
}
