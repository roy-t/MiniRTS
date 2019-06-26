using System;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Containers
{
    public interface IComponentContainer
    {
        int Count { get; }
        void RemoveAllOwnedBy(Entity entity);
        Type GetComponentType();
        IComponent this[int index] { get; }
    }

    public interface IComponentContainer<T> : IComponentContainer
        where T : IComponent
    {
        void Add(T item);
        new T this[int index] { get; }
        bool Remove(T item);        
    }
}