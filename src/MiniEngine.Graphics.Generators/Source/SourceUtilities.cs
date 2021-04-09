namespace MiniEngine.Graphics.Generators.Source
{
    public static class SourceUtilities
    {
        public static string CapitalizeFirstLetter(string text)
            => char.ToUpper(text[0]) + text.Substring(1);
    }
}
