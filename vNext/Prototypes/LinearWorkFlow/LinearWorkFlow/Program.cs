using System;

namespace LinearWorkFlow
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameLoop())
                game.Run();
        }
    }
}
