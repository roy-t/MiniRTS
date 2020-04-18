using System;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Containers
{
    public interface IComponentContainer
    {
        int Count { get; }

        void Clear();

        Type GetComponentType();

        IComponent this[int index] { get; }

        void Remove(Entity entity);

        IComponent Get(Entity entity);
    }

    public interface IComponentContainer<T> : IComponentContainer
        where T : IComponent
    {
        void Add(T item);

        new T Get(Entity entity);

        new T this[int index] { get; }
    }
}