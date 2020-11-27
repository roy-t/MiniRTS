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
        public IReadOnlyList<T> All { get; }

        public IReadOnlyList<T> New { get; }

        public IReadOnlyList<T> Changed { get; }

        public IReadOnlyList<T> Unchanged { get; }

        public IReadOnlyList<T> Removed { get; }

        public T Get(int index);

        public T Get(Entity entity);

        public void Add(T component);
    }

    [ComponentContainer]
    public sealed class ComponentContainer<T> : IComponentContainer<T>
        where T : AComponent
    {
        private readonly List<T> AllComponents;

        private readonly List<T> NewComponents;
        private readonly List<T> ChangedComponents;
        private readonly List<T> UnchangedComponents;
        private readonly List<T> RemovedComponents;

        private readonly Dictionary<Entity, T> Components;

        public ComponentContainer()
        {
            this.AllComponents = new List<T>();

            this.NewComponents = new List<T>();
            this.ChangedComponents = new List<T>();
            this.UnchangedComponents = new List<T>();
            this.RemovedComponents = new List<T>();

            this.Components = new Dictionary<Entity, T>();
        }

        public Type ComponentType => typeof(T);

        public IReadOnlyList<T> All => this.AllComponents;

        public IReadOnlyList<T> New => this.NewComponents;

        public IReadOnlyList<T> Changed => this.ChangedComponents;

        public IReadOnlyList<T> Unchanged => this.UnchangedComponents;

        public IReadOnlyList<T> Removed => this.RemovedComponents;

        public T Get(int index)
            => this.AllComponents[index];

        public T Get(Entity entity)
            => this.Components[entity];

        // TODO: Replace once https://github.com/dotnet/runtime/issues/45037 is fixed
        public AComponent GetComponent(int index) => this.Get(index);

        // TODO: Replace once https://github.com/dotnet/runtime/issues/45037 is fixed
        public AComponent GetComponent(Entity entity) => this.Get(entity);

        public int Count => this.AllComponents.Count;

        public void Flush()
        {
            this.NewComponents.Clear();
            this.ChangedComponents.Clear();
            this.UnchangedComponents.Clear();

            for (var i = this.AllComponents.Count - 1; i >= 0; i--)
            {
                var component = this.AllComponents[i];

                if (component.ChangeState.CurrentState == LifetimeState.Removed)
                {
                    this.AllComponents.RemoveAt(i);
                    this.Components.Remove(component.Entity);
                }
                else
                {
                    component.ChangeState.Next();
                    switch (component.ChangeState.CurrentState)
                    {
                        case LifetimeState.Created:
                            throw new InvalidOperationException();
                        case LifetimeState.New:
                            this.NewComponents.Add(component);
                            break;

                        case LifetimeState.Changed:
                            this.ChangedComponents.Add(component);
                            break;

                        case LifetimeState.Unchanged:
                            this.UnchangedComponents.Add(component);
                            break;

                        case LifetimeState.Removed:
                            this.RemovedComponents.Add(component);
                            break;
                    }
                }
            }
        }

        public void Add(T component)
        {
            this.AllComponents.Add(component);
            this.Components.Add(component.Entity, component);
        }

        public bool Contains(Entity entity)
            => this.Components.ContainsKey(entity);
    }
}
