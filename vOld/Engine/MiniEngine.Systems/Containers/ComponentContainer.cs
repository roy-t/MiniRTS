﻿using System;
using System.Collections.Generic;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Containers
{
    public sealed class ComponentContainer<T> : IComponentContainer<T>
        where T : IComponent
    {
        private readonly List<T> Components;
        private readonly Dictionary<Entity, T> LookUp;

        public ComponentContainer()
        {
            this.Components = new List<T>();
            this.LookUp = new Dictionary<Entity, T>();
        }

        public void Add(T item)
        {
            this.Components.Add(item);
            this.LookUp.Add(item.Entity, item);
        }

        public void Remove(Entity entity)
        {
            if (this.LookUp.TryGetValue(entity, out var component))
            {
                this.Components.Remove(component);
                this.LookUp.Remove(entity);
            }
        }

        public T Get(Entity entity) => this.LookUp[entity];

        IComponent IComponentContainer.Get(Entity entity) => this.Get(entity);

        public bool TryGet(Entity entity, out T component) => this.LookUp.TryGetValue(entity, out component);

        bool IComponentContainer.TryGet(Entity entity, out IComponent component)
        {
            if (this.TryGet(entity, out var c))
            {
                component = c;
                return true;
            }

            component = default;
            return false;
        }

        public int Count => this.Components.Count;

        public void Clear() => this.Components.Clear();

        public T this[int index] => this.Components[index];

        IComponent IComponentContainer.this[int index] => this[index];

        public Type GetComponentType() => typeof(T);
    }
}