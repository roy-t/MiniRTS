using System;
using MiniEngine.Configuration;

namespace MiniEngine.Editor
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var injector = new Injector();
            using var game = injector.Create<GameLoop>();
            game.Run();
        }
    }
}
