using MiniEngine.Systems.Components;

namespace MiniEngine.Systems.Next
{
    public interface ISystem { }

    public interface ISystemWithoutComponent : ISystem
    {
        public void Process();
    }


    public interface ISystem<T0> : ISystem
        where T0 : IComponent
    {
        public void Process(T0 component);
    }

    public interface ISystem<T0, T1> : ISystem
       where T0 : IComponent
        where T1 : IComponent
    {
        public void Process(T0 component, T1 component2);
    }
}
