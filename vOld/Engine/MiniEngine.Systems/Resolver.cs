using System;
using System.Collections.Generic;

namespace MiniEngine.Systems
{
    public sealed class Resolver<T>
    {
        private readonly Dictionary<Type, T> Collection;

        public Resolver(IEnumerable<T> collection)
        {
            this.Collection = new Dictionary<Type, T>();
            foreach (var item in collection)
            {
                this.Collection.Add(item.GetType(), item);
            }
        }

        public V Get<V>()
            where V : T => (V)this.Collection[typeof(V)];


        private struct TypeInfo
        {
            public TypeInfo(Type baseInterface, Type genericArgument)
            {
                this.BaseInterface = baseInterface;
                this.GenericArgument = genericArgument;
            }

            public Type BaseInterface { get; }
            public Type GenericArgument { get; }
        }
    }
}
