namespace MiniEngine.Configuration
{
    public interface IInstanceFactory
    {
    }

    public interface IInstanceFactory<T, I> : IInstanceFactory
    {
        T Build(I input);
    }
}