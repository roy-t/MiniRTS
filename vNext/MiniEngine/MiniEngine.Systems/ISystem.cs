namespace MiniEngine.Systems
{
    public interface ISystemBase { }

    public interface ISystem : ISystemBase
    {
        public void Process();
    }


    public interface ISystem<T0> : ISystemBase
        where T0 : IComponent
    {
        public void Process(T0 component);
    }

    public interface ISystem<T0, T1> : ISystemBase
       where T0 : IComponent
        where T1 : IComponent
    {
        public void Process(T0 component, T1 component2);
    }
}
