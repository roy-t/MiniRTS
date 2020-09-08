using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiniEngine.Systems.Components;

namespace MiniEngine.Systems
{
    public interface ISystemBinding
    {
        public void Process();
    }

    public class SystemBindingWithoutComponents : ISystemBinding
    {
        private readonly MethodInfo ProcessDelegate;
        private readonly ISystem System;

        public SystemBindingWithoutComponents(MethodInfo processDelegate, ISystem system)
        {
            this.ProcessDelegate = processDelegate;
            this.System = system;
        }

        public void Process()
            => this.ProcessDelegate.Invoke(this.System, null);

        public override string ToString()
            => $"{this.System.GetType().Name}";
    }

    public class SystemBindingWithOneComponent : ISystemBinding
    {
        private readonly MethodInfo ProcessDelegate;
        private readonly ISystem System;
        private readonly IComponentContainer ComponentContainer;
        private readonly object[] Parameters;

        public SystemBindingWithOneComponent(MethodInfo processDelegate, ISystem system, IComponentContainer componentContainer)
        {
            this.ProcessDelegate = processDelegate;
            this.System = system;
            this.ComponentContainer = componentContainer;
            this.Parameters = new object[1];
        }

        public void Process()
        {
            for (var i = 0; i < this.ComponentContainer.Count; i++)
            {
                this.Parameters[0] = this.ComponentContainer[i];
                this.ProcessDelegate.Invoke(this.System, this.Parameters);
            }
        }

        public override string ToString()
            => $"{this.System.GetType().Name}<{this.ComponentContainer.GetType().GenericTypeArguments[1].Name}>";
    }

    public class SystemBindingWithManyComponents : ISystemBinding
    {
        private readonly MethodInfo ProcessDelegate;
        private readonly ISystem System;
        private readonly IReadOnlyList<IComponentContainer> ComponentContainers;
        private readonly object[] Parameters;

        public SystemBindingWithManyComponents(MethodInfo processDelegate, ISystem system, IReadOnlyList<IComponentContainer> componentContainers)
        {
            this.ProcessDelegate = processDelegate;
            this.System = system;
            this.ComponentContainers = componentContainers;
            this.Parameters = new object[componentContainers.Count];
        }

        public void Process()
        {
            for (var c = 0; c < this.ComponentContainers[0].Count; c++)
            {
                var primaryComponent = this.ComponentContainers[0][c];
                this.Parameters[0] = primaryComponent;
                var entity = primaryComponent.Entity;
                for (var i = 1; i < this.Parameters.Length; i++)
                {
                    var componentContainer = this.ComponentContainers[i];
                    this.Parameters[i] = componentContainer[entity];

                }

                this.ProcessDelegate.Invoke(this.System, this.Parameters);
            }
        }

        public override string ToString()
            => $"{this.System.GetType().Name}<{string.Join(", ", this.ComponentContainers.Select(c => c.GetType().GenericTypeArguments[1].Name))}>";
    }
}
