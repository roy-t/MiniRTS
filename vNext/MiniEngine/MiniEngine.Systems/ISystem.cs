namespace MiniEngine.Systems
{
    public interface ISystemBase { }

    public interface ISystem : ISystemBase
    {
        public void Process();
    }


    public interface ISystem<T0> : ISystemBase
        where T0 : AComponent
    {
        public void Process(T0 component);
    }

    public interface ISystem<T0, T1> : ISystemBase
       where T0 : AComponent
        where T1 : AComponent
    {
        public void Process(T0 component, T1 component2);
    }
}
