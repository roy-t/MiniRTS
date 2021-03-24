namespace MiniEngine.Graphics.Mutators
{
    public interface IMutatorFunction<T>
        where T : struct
    {
        T Update(float elapsed, T value);
    }
}
