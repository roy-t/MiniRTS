using System;
using System.Collections.Generic;

namespace MiniEngine.Systems
{
    internal sealed class MultiValueDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> Dictionary;

        public MultiValueDictionary()
        {
            this.Dictionary = new Dictionary<TKey, List<TValue>>();
        }

        public void Add(TKey key, TValue value)
        {
            if (this.Dictionary.TryGetValue(key, out var values))
            {
                values.Add(value);
            }
            else
            {
                var list = new List<TValue>
                {
#pragma warning disable IDE0009 // Member access should be qualified.
                    value
#pragma warning restore IDE0009 // Member access should be qualified.
                };
                this.Dictionary.Add(key, list);
            }
        }

        public IList<TValue> Get(TKey key)
        {            
            if(this.Dictionary.TryGetValue(key, out var values))
            {
                return values;
            }

            return new List<TValue>(0);
        }

        internal void Remove(TKey key) => this.Dictionary.Remove(key);
    }
}
