using System;
using System.Linq;

namespace MiniEngine.Editor
{
    public static class StartupArguments
    {
        public static string GameLoopType => GetArgumentValueOrDefault("--gameloop", "MiniEngine.Editor.GameLoop");

        private static bool IsPresent(string argument)
        {
            var args = Environment.GetCommandLineArgs();
            return args.Any(a => a.Equals(argument, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetArgumentValueOrDefault(string argument, string def)
        {
            var value = GetArgumentValue(argument);
            if (string.IsNullOrEmpty(value))
            {
                return def;
            }

            return value;
        }

        private static string GetArgumentValue(string argument)
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals(argument, StringComparison.OrdinalIgnoreCase))
                {
                    return Unquote(args[i + 1]);
                }
            }

            return string.Empty;
        }

        private static string Unquote(string value)
        {
            if (value.StartsWith('"') && value.EndsWith('"'))
            {
                return value[1..^1];
            }

            return value;
        }
    }
}
