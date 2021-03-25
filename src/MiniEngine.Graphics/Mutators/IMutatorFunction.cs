namespace MiniEngine.Graphics.Mutators
{
    public interface IMutatorFunction<T>
    {
        void Update(float elapsed, T value);
    }
}
