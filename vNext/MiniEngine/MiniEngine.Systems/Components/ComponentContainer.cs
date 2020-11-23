using System;
using System.Collections.Generic;
using MiniEngine.Configuration;

namespace MiniEngine.Systems.Components
{
    public interface IComponentContainer
    {
        public Type ComponentType { get; }

        public abstract int Count { get; }

        public abstract void Flush();

        public AComponent GetComponent(int index);

        public AComponent GetComponent(Entity entity);

        public abstract bool Contains(Entity entity);

        public IComponentContainer<T> Specialize<T>()
            where T : AComponent
            => (IComponentContainer<T>)this;
    }

    public interface IComponentContainer<T> : IComponentContainer
        where T : AComponent
    {
        public IReadOnlyList<T> New { get; }

        public IReadOnlyList<T> Changed { get; }

        public IReadOnlyList<T> Unchanged { get; }

        public T Get(int index);

        public T Get(Entity entity);

        public void Add(T component);

        public void MarkChanged(T component);
    }

    [ComponentContainer]
    public sealed class ComponentContainer<T> : IComponentContainer<T>
        where T : AComponent
    {
        private readonly List<T> NewComponents;
        private readonly List<T> ChangedComponents;
        private readonly List<T> UnchangedComponents;
        private readonly Dictionary<Entity, T> Components;

        private readonly List<T> ToBeChangedComponents;

        public ComponentContainer()
        {
            this.NewComponents = new List<T>(); this.ChangedComponents =
new List<T>(); this.UnchangedComponents = new List<T>(); this.Components = new
Dictionary<Entity, T>();

            this.ToBeChangedComponents = new List<T>();
        }

        public Type ComponentType => typeof(T);

        public IReadOnlyList<T> New => this.NewComponents;

        public IReadOnlyList<T> Changed => this.ChangedComponents;

        public IReadOnlyList<T> Unchanged => this.UnchangedComponents;

        public T Get(int index)
        {
            if (index < this.NewComponents.Count)
            {
                return this.NewComponents[index];
            }

            index -= this.NewComponents.Count; if (index < this.ChangedComponents.Count)
            {
                return this.ChangedComponents[index];
            }

            index -= this.ChangedComponents.Count; return this.UnchangedComponents[index];
        }

        public T Get(Entity entity) => this.Components[entity];

        // TODO: Replace once https://github.com/dotnet/runtime/issues/45037 is fixed
        public AComponent GetComponent(int index) => this.Get(index);

        // TODO: Replace once https://github.com/dotnet/runtime/issues/45037 is fixed
        public AComponent GetComponent(Entity entity) => this.Get(entity);

        public int Count => this.Components.Count;

        public void Flush()
        {
            for (var i = 0; i < this.NewComponents.Count; i++)
            {
                this.UnchangedComponents.Add(this.NewComponents[i]);
            }
            this.NewComponents.Clear();

            for (var i = 0; i < this.ChangedComponents.Count; i++)
            {
                this.UnchangedComponents.Add(this.ChangedComponents[i]);
            }
            this.ChangedComponents.Clear();

            for (var i = 0; i < this.ToBeChangedComponents.Count; i++)
            {
                this.ChangedComponents.Add(this.ToBeChangedComponents[i]);
            }
        }

        public void Add(T component)
        {
            this.NewComponents.Add(component);
            this.Components.Add(component.Entity, component);
        }

        public void MarkChanged(T component)
        {
            // TODO: we can optimized this by creating a HashSet
            //that has a garbage free way to iterate over all contained elements
            if (!this.ToBeChangedComponents.Contains(component))
            {
                this.ToBeChangedComponents.Add(component);
            }
        }

        public bool Contains(Entity entity)
            => this.Components.ContainsKey(entity);
    }
}
