using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Next;

namespace MiniEngine.Pipeline.Next
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
            => $"{this.System.GetType().Name}<>";
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
                var component = this.ComponentContainer[i];
                this.Parameters[0] = component;
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
            for (var i = 0; i < this.ComponentContainers[0].Count; i++)
            {
                var primaryComponent = this.ComponentContainers[0][i];
                this.Parameters[0] = primaryComponent;
                var entity = primaryComponent.Entity;
                for (var j = 1; j < this.Parameters.Length; j++)
                {
                    var componentContainer = this.ComponentContainers[j];
                    this.Parameters[j] = componentContainer[entity];

                }

                this.ProcessDelegate.Invoke(this.System, this.Parameters);
            }
        }

        public override string ToString()
            => $"{this.System.GetType().Name}<{string.Join(", ", this.ComponentContainers.Select(c => c.GetType().GenericTypeArguments[1].Name))}>";
    }
}
