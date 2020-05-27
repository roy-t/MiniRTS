using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public interface IManeuver
    {
        void Update(Seconds elapsed);

        bool Completed { get; }
    }
}
