using Microsoft.Xna.Framework;
using MiniEngine.Configuration;

namespace MiniEngine.Editor
{
    [Service]
    internal sealed class GameTimer
    {
        private readonly Game Game;

        public GameTimer(Game game)
        {
            this.Game = game;
        }

        public bool IsFixedTimeStep
        {
            get => this.Game.IsFixedTimeStep;
            set => this.Game.IsFixedTimeStep = value;
        }
    }
}
