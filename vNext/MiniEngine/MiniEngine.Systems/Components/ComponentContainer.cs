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
        private readonly Dictionary<Entity, T> Components;

        public ComponentContainer()
        {
            this.AllComponents = new List<T>();
            this.NewComponents = new List<T>();
            this.ChangedComponents = new List<T>();
            this.UnchangedComponents = new List<T>(); this.Components = new Dictionary<Entity, T>();
        }

        public Type ComponentType => typeof(T);

        public IReadOnlyList<T> All => this.AllComponents;

        public IReadOnlyList<T> New => this.NewComponents;

        public IReadOnlyList<T> Changed => this.ChangedComponents;

        public IReadOnlyList<T> Unchanged => this.UnchangedComponents;

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

            for (var i = 0; i < this.AllComponents.Count; i++)
            {
                var component = this.AllComponents[i];
                component.ChangeState.Next();

                switch (component.ChangeState.CurrentState)
                {
                    case ChangeState.Initialized:
                        throw new InvalidOperationException();
                    case ChangeState.New:
                        this.NewComponents.Add(component);
                        break;

                    case ChangeState.Changed:
                        this.ChangedComponents.Add(component);
                        break;

                    case ChangeState.Unchanged:
                        this.UnchangedComponents.Add(component);
                        break;
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
