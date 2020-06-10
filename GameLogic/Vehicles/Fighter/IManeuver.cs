using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public interface IManeuver
    {
        void Update(Pose pose, Seconds elapsed);

        bool Completed { get; }
    }
}
