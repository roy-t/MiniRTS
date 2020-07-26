using MiniEngine.Systems.Annotations;

namespace GameLogic.BluePrints
{
    public sealed class AsteroidBluePrint
    {
        public AsteroidBluePrint()
        {
            this.Radius = 1.0f;
            this.Subdivisions = 0;
        }

        [Editor(nameof(Radius))]
        public float Radius { get; set; }


        [Editor(nameof(Subdivisions), nameof(Subdivisions), 0.0f, 10000.0f)]
        public int Subdivisions { get; set; }
    }
}
