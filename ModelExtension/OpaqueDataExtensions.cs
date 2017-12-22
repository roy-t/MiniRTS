using Microsoft.Xna.Framework.Content.Pipeline;

namespace ModelExtension
{
    public static class OpaqueDataExtensions
    {
        public static T GetValue<T>(this OpaqueDataDictionary dictionary, string key, T fallback)
        {
            if (dictionary.TryGetValue(key, out object value))
            {
                return (T) value;
            }

            return fallback;
        }
    }
}
